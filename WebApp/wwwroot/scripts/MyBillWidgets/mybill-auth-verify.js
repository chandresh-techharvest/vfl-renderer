// MyBill Authentication - Verification Script
// Copy and paste this into browser console AFTER logging in via /mybill-login

(async function() {



    const cookies = document.cookie.split(';').map(c => c.trim());
    const myBillCookie = cookies.find(c => c.startsWith('MyBillAuthCookie='));
    
    if (!myBillCookie) {
        return;
    }

    try {
        const response = await fetch('/api/MyBillDashboard/GetProfileInformation', {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include'
        });


        if (response.status === 200) {
            
            const data = await response.json();
            
            return true;

        } else if (response.status === 401) {
            // Unauthorized
        } else if (response.status === 403) {
            // Forbidden
        }

    } catch (error) {
        // Request failed
    }

    
    return false;

})();
