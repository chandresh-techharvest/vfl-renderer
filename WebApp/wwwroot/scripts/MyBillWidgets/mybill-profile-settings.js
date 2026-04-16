/**
 * MyBill Profile Settings Widget
 * Handles profile updates and password changes
 */

(function () {
    'use strict';

    // Configuration
    const CONFIG = {
        API_BASE: '/api/MyBillProfileSettings',
        PASSWORD_MIN_LENGTH: 8,
        PASSWORD_MAX_LENGTH: 100
    };

    // DOM Elements
    let elements = {
        widget: null,
        // Profile form elements
        profileForm: null,
        accountName: null,
        contactFullName: null,
        phoneNumber: null,
        email: null,
        selectedBan: null,
        updateProfileBtn: null,
        cancelProfileBtn: null,
        profileAlert: null,
        // Password form elements
        passwordForm: null,
        newPassword: null,
        confirmPassword: null,
        updatePasswordBtn: null,
        cancelPasswordBtn: null,
        passwordAlert: null
    };

    // State
    let state = {
        isUpdating: false,
        originalProfileData: {}
    };

    /**
     * Initialize the widget
     */
    function init() {
        elements.widget = document.getElementById('myBillProfileSettings');
        if (!elements.widget) {
            return;
        }

        // Cache DOM elements
        cacheElements();

        // Setup event listeners
        setupEventListeners();

        // mybill-auth-refresh.js intercepts ALL /api/MyBill* fetch calls.
        // If the JWT is expired (401), it automatically refreshes via the
        // backend refresh token (7-day cookie) and retries the request.
        // No eager refresh needed — just load data directly.
        loadProfileData();
    }

    /**
     * Cache DOM elements
     */
    function cacheElements() {
        // Profile form
        elements.profileForm = document.getElementById('profileUpdateForm');
        elements.accountName = document.getElementById('accountName');
        elements.contactFullName = document.getElementById('contactFullName');
        elements.phoneNumber = document.getElementById('phoneNumber');
        elements.email = document.getElementById('email');
        elements.selectedBan = document.getElementById('selectedBan');
        elements.updateProfileBtn = document.getElementById('updateProfileBtn');
        elements.cancelProfileBtn = document.getElementById('cancelProfileBtn');
        elements.profileAlert = document.getElementById('profileAlert');

        // Password form
        elements.passwordForm = document.getElementById('passwordUpdateForm');
        elements.newPassword = document.getElementById('newPassword');
        elements.confirmPassword = document.getElementById('confirmPassword');
        elements.updatePasswordBtn = document.getElementById('updatePasswordBtn');
        elements.cancelPasswordBtn = document.getElementById('cancelPasswordBtn');
        elements.passwordAlert = document.getElementById('passwordAlert');
    }

    /**
     * Setup event listeners
     */
    function setupEventListeners() {
        // Profile form buttons
        if (elements.updateProfileBtn) {
            elements.updateProfileBtn.addEventListener('click', handleProfileUpdate);
        }
        
        if (elements.cancelProfileBtn) {
            elements.cancelProfileBtn.addEventListener('click', handleProfileCancel);
        }

        // Password form buttons
        if (elements.updatePasswordBtn) {
            elements.updatePasswordBtn.addEventListener('click', handlePasswordUpdate);
        }
        
        if (elements.cancelPasswordBtn) {
            elements.cancelPasswordBtn.addEventListener('click', handlePasswordCancel);
        }
    }

    /**
     * Load profile data from API
     * @param {boolean} refresh - If true, forces a fresh load from server (bypasses cache)
     */
    async function loadProfileData(refresh = false) {
        try {
            // Get selected BAN from dashboard cookie
            const selectedBan = getSelectedBanFromCookie();
            if (!selectedBan) {
                showProfileAlert('Unable to load profile data. Please select an account.', 'warning');
                return;
            }

            if (elements.selectedBan) {
                elements.selectedBan.value = selectedBan;
            }

            // Add refresh parameter to bypass cache if requested
            const url = refresh 
                ? `${CONFIG.API_BASE}/GetProfileInfo/${selectedBan}?refresh=true`
                : `${CONFIG.API_BASE}/GetProfileInfo/${selectedBan}`;

            // Get profile data from endpoint
            const response = await fetch(url);
            
            if (!response.ok) {
                showProfileAlert('Failed to load profile data', 'danger');
                return;
            }

            const result = await response.json();

            if (result.isSuccess && result.data) {
                populateProfileForm(result.data);
                // Store original data for cancel functionality
                state.originalProfileData = {
                    accountName: result.data.accountName || '',
                    contactFullName: result.data.contactFullName || '',
                    phoneNumber: result.data.phoneNumber || '',
                    email: result.data.email || ''
                };
            } else {
                showProfileAlert('Failed to load profile data', 'danger');
            }
        } catch (error) {
            showProfileAlert('Failed to load profile data', 'danger');
        }
    }

    /**
     * Populate profile form with data
     */
    function populateProfileForm(data) {
        if (elements.accountName) elements.accountName.value = data.accountName || '';
        if (elements.contactFullName) elements.contactFullName.value = data.contactFullName || '';
        if (elements.phoneNumber) elements.phoneNumber.value = data.phoneNumber || '';
        if (elements.email) elements.email.value = data.email || '';
        
    }

    /**
     * Get selected BAN from cookie
     */
    function getSelectedBanFromCookie() {
        const cookieValue = getCookie('mybillData');
        if (!cookieValue) return null;

        try {
            const data = JSON.parse(decodeURIComponent(cookieValue));
            const selectedBan = data.BanAccounts?.find(b => b.IsSelected);
            return selectedBan?.Number || null;
        } catch (error) {
            return null;
        }
    }

    /**
     * Get cookie value by name
     */
    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
        return null;
    }

    /**
     * Handle profile update
     */
    async function handleProfileUpdate(e) {
        e.preventDefault();

        if (state.isUpdating) {
            return;
        }

        // Validate form
        if (!validateProfileForm()) {
            return;
        }

        // Get form data
        const selectedBan = elements.selectedBan.value;
        const accountName = elements.accountName.value.trim();
        const contactFullName = elements.contactFullName.value.trim();
        const phoneNumber = elements.phoneNumber.value.trim();
        const email = elements.email.value.trim();


        // Prepare request - all fields are required by backend
        const requestData = {
            accountName: accountName,
            contactFullName: contactFullName,
            contactPhoneNumber: phoneNumber,
            email: email
        };

        try {
            state.isUpdating = true;
            setButtonLoading(elements.updateProfileBtn, true);
            hideProfileAlert();

            const response = await fetch(`${CONFIG.API_BASE}/UpdateProfile/${selectedBan}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            const result = await response.json();

            if (response.ok && result.isSuccess) {
                showProfileAlert('Profile updated successfully!', 'success');
                
                // Reload profile data to get fresh data from server
                await loadProfileData(true); // Pass true to force refresh
            } else {
                // Handle validation errors
                if (result.errors) {
                    const errorMessages = [];
                    for (const field in result.errors) {
                        errorMessages.push(...result.errors[field]);
                    }
                    showProfileAlert(errorMessages.join(' '), 'danger');
                } else {
                    showProfileAlert(result.message || 'Failed to update profile', 'danger');
                }
            }
        } catch (error) {
            showProfileAlert('An error occurred while updating profile', 'danger');
        } finally {
            state.isUpdating = false;
            setButtonLoading(elements.updateProfileBtn, false);
        }
    }

    /**
     * Handle profile cancel
     */
    function handleProfileCancel(e) {
        e.preventDefault();
        
        // Reset form to original values
        if (elements.accountName) elements.accountName.value = state.originalProfileData.accountName || '';
        if (elements.contactFullName) elements.contactFullName.value = state.originalProfileData.contactFullName || '';
        if (elements.phoneNumber) elements.phoneNumber.value = state.originalProfileData.phoneNumber || '';
        if (elements.email) elements.email.value = state.originalProfileData.email || '';

        hideProfileAlert();
    }

    /**
     * Validate profile form
     */
    function validateProfileForm() {
        const accountName = elements.accountName.value.trim();
        const contactFullName = elements.contactFullName.value.trim();
        const phoneNumber = elements.phoneNumber.value.trim();
        const email = elements.email.value.trim();

        // All fields are required
        if (!accountName) {
            showProfileAlert('Account Name is required', 'warning');
            return false;
        }

        if (!contactFullName) {
            showProfileAlert('Contact Full Name is required', 'warning');
            return false;
        }

        if (!phoneNumber) {
            showProfileAlert('Phone Number is required', 'warning');
            return false;
        }

        if (!email) {
            showProfileAlert('Email address is required', 'warning');
            return false;
        }

        // Basic email validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showProfileAlert('Please enter a valid email address', 'warning');
            return false;
        }

        return true;
    }

    /**
     * Handle password update
     */
    async function handlePasswordUpdate(e) {
        e.preventDefault();

        if (state.isUpdating) {
            return;
        }

        // Validate form
        if (!validatePasswordForm()) {
            return;
        }

        const selectedBan = elements.selectedBan.value;
        const newPassword = elements.newPassword.value;
        const confirmPassword = elements.confirmPassword.value;


        // Prepare request
        const requestData = {
            newPassword: newPassword,
            confirmPassword: confirmPassword
        };

        try {
            state.isUpdating = true;
            setButtonLoading(elements.updatePasswordBtn, true);
            hidePasswordAlert();

            const response = await fetch(`${CONFIG.API_BASE}/UpdatePassword/${selectedBan}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            const result = await response.json();

            if (response.ok && result.isSuccess) {
                showPasswordAlert('Password updated successfully!', 'success');
                // Clear password fields
                elements.newPassword.value = '';
                elements.confirmPassword.value = '';
            } else {
                showPasswordAlert(result.message || 'Failed to update password', 'danger');
            }
        } catch (error) {
            showPasswordAlert('An error occurred while updating password', 'danger');
        } finally {
            state.isUpdating = false;
            setButtonLoading(elements.updatePasswordBtn, false);
        }
    }

    /**
     * Handle password cancel
     */
    function handlePasswordCancel(e) {
        e.preventDefault();
        
        // Clear password fields
        if (elements.newPassword) elements.newPassword.value = '';
        if (elements.confirmPassword) elements.confirmPassword.value = '';
        hidePasswordAlert();
    }

    /**
     * Validate password form
     */
    function validatePasswordForm() {
        const newPassword = elements.newPassword.value;
        const confirmPassword = elements.confirmPassword.value;

        if (!newPassword || !confirmPassword) {
            showPasswordAlert('Both password fields are required', 'warning');
            return false;
        }

        if (newPassword !== confirmPassword) {
            showPasswordAlert('Passwords do not match', 'warning');
            return false;
        }

        // Validate password strength
        if (newPassword.length < CONFIG.PASSWORD_MIN_LENGTH) {
            showPasswordAlert(`Password must be at least ${CONFIG.PASSWORD_MIN_LENGTH} characters long`, 'warning');
            return false;
        }

        if (newPassword.length > CONFIG.PASSWORD_MAX_LENGTH) {
            showPasswordAlert(`Password must not exceed ${CONFIG.PASSWORD_MAX_LENGTH} characters`, 'warning');
            return false;
        }

        // Check for uppercase letter
        if (!/[A-Z]/.test(newPassword)) {
            showPasswordAlert('Password must contain at least one uppercase letter', 'warning');
            return false;
        }

        // Check for special character
        if (!/[!@#$%^&*(),.?":{}|<>]/.test(newPassword)) {
            showPasswordAlert('Password must contain at least one special character', 'warning');
            return false;
        }

        return true;
    }

    /**
     * Show profile alert
     */
    function showProfileAlert(message, type) {
        if (!elements.profileAlert) return;
        elements.profileAlert.textContent = message;
        elements.profileAlert.className = `alert alert-${type}`;
        elements.profileAlert.classList.remove('d-none');
    }

    /**
     * Hide profile alert
     */
    function hideProfileAlert() {
        if (!elements.profileAlert) return;
        elements.profileAlert.classList.add('d-none');
    }

    /**
     * Show password alert
     */
    function showPasswordAlert(message, type) {
        if (!elements.passwordAlert) return;
        elements.passwordAlert.textContent = message;
        elements.passwordAlert.className = `alert alert-${type}`;
        elements.passwordAlert.classList.remove('d-none');
    }

    /**
     * Hide password alert
     */
    function hidePasswordAlert() {
        if (!elements.passwordAlert) return;
        elements.passwordAlert.classList.add('d-none');
    }

    /**
     * Set button loading state
     */
    function setButtonLoading(button, isLoading) {
        if (!button) return;

        if (isLoading) {
            button.disabled = true;
            button.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Updating...';
        } else {
            button.disabled = false;
            button.innerHTML = '<i class="ri-refresh-line me-1"></i> Update';
        }
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
