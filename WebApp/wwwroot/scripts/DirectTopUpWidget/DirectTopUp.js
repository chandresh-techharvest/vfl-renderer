var topupform = document.getElementById("topupform");
var topupreview = document.getElementById("topupreview");
var topupsucess = document.getElementById("topupsucess");
var btnContinue = document.getElementById("btnContinue");
var phoneinput = document.getElementById("mobile_number");
var mobilenumberreview = document.getElementById("mobilenumberreview");
var form = document.getElementById("DirectTopup"); 
var pinreview = document.getElementById("pinreview");
var rechargepin = document.getElementById("rechargepin");

var invalidDataAttr = "data-sf-invalid";
var invalidClassElement = document.querySelector('[data-sf-invalid]');
var classInvalidValue = invalidClassElement?.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
function processCssClass(str) {
    var classList = str.split(" ");
    return classList;
}

btnContinue.addEventListener('click', async function (e) {
    e.preventDefault();
   
    let isValid = true;
    isValid = await validateForm();
    if (isValid) {
        let myCookie = await getDecryptedCookie();
        var nameReview = document.getElementById("nameReview");
        var emailReview = document.getElementById("emailReview");
        var countryReview = document.getElementById("countryReview");
        if (myCookie) {
            const luData = myCookie;
            nameReview.innerText = "Name: " + luData.firstName + " " + luData.lastName;
            emailReview.innerText = "Email: " + luData.email;            
        }


        mobilenumberreview.innerText = "Mobile number: " + phoneinput.value;
        pinreview.innerText = "PIN: " + rechargepin.value;
        topupform.classList.add('d-none');
        topupreview.classList.remove('d-none');        

        var btnBack = document.getElementById("btnBack");
        var btnConfirm = document.getElementById("btnConfirm");
        btnBack.addEventListener("click", function () {
            topupform.classList.remove('d-none');
            topupreview.classList.add('d-none');            
        });
        btnConfirm.addEventListener("click", function () {
            formSubmitHandler();
        });

    }
});

var formSubmitHandler = function () {
    var agreeCheck = document.getElementById("agreeCheck");
    if (agreeCheck.checked != true) {
        invalidateElement(agreeCheck);
        AppendError(agreeCheck.name + ' This field is required.');
        return false;
    }
    var url = "/api/DirectTopUp/SendRequest"
    var model = {};
    model.pin = rechargepin.value;
    model.phoneNumber = phoneinput.value;
    window.fetch(url, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
        .then(response => response.json())
        .then(res => {
            console.log(res);
            if (res.statusCode >= 200 && res.statusCode < 400) {
                if (res.data != null && (res.data.isToppedUp)) {                    
                    
                    var successDate = document.getElementById("successDate");
                    var successOrderNumber = document.getElementById("successOrderNumber");
                    var successEmail = document.getElementById("successEmail");
                    var successNumber = document.getElementById("successNumber");
                    var successAmount = document.getElementById("successAmount");
                    successDate.innerText = new Date(res.data.date).toLocaleDateString();
                    successOrderNumber.innerText = res.data.reference;
                    successEmail.innerText = res.data.email;
                    successNumber.innerText = res.data.number;
                    //successAmount.innerText = res.data.amount;
                    // ✅ Show meaningful message
                    topupform.classList.add('d-none');
                    topupreview.classList.add('d-none');
                    topupsucess.classList.remove('d-none');
                }
                else {
                    var failDate = document.getElementById("failDate");
                    var failOrderNumber = document.getElementById("failOrderNumber");
                    var failEmail = document.getElementById("failEmail");
                    var failNumber = document.getElementById("failNumber");
                    var failAmount = document.getElementById("failAmount");

                    failDate.innerText = new Date(res.data.date).toLocaleDateString();
                    failEmail.innerText = res.data.email;
                    failNumber.innerText = res.data.number;
                    //failAmount.innerText = res.data.amount;
                    failOrderNumber.innerText = res.data.reference;

                    // ✅ Show meaningful message
                    topupform.classList.add('d-none');
                    topupreview.classList.add('d-none');
                    topupfail.classList.remove('d-none');

                    btnFailBack.addEventListener("click", function () {
                        topupform.classList.remove('d-none');
                        topupfail.classList.add('d-none');
                    });
                }

            }
            else {

                res.developerErrorMessage != null ? showError(res.developerErrorMessage) : showError(res.title)

                showElement(Element);
                invalidateElement(Element);                
            }
        })
        .catch(error => {
            showError(error);            
            invalidateElement(Element);            
            console.error('Error fetching data:', error);
        });

}

var invalidateElement = function (element) {

    if (!element || !element.classList) return;

    if (Array.isArray(classInvalid) && classInvalid.length > 0) {
        element.classList.add(...classInvalid);
    }

    element.setAttribute(invalidDataAttr, "");
};

var PhoneBlurHandler = async function (form, onSuccess, Element) {
    const phoneNumber = phoneinput.value;
    let isValid = true;
    if (!phoneNumber) {
        return false;
    }

    try {
        const valResponse = await fetch("/api/Validation/CheckNumberIsValid_AllowInactiveNumber", {
            method: 'POST',
            body: JSON.stringify(phoneNumber),
            headers: { 'Content-Type': 'application/json' }
        });
        const valRes = await valResponse.json();

        if (valRes.data?.isNumberValid === false) {
            isValid = false;
            invalidateElement(phoneinput);
        } else {

        }

    } catch (err) {
        console.error("Validation error:", err);
        // Optional: btnRegister.disabled = false; // allow retry on network failure
    }
    return isValid;
};
var resetValidationErrors = function (parentElement) {

    var invalidElements = parentElement.querySelectorAll('[' + invalidDataAttr + ']');

    invalidElements.forEach(function (el) {
        if (classInvalid) {
            el.classList.remove(...classInvalid);
        }
        el.removeAttribute(invalidDataAttr);
    });

    var errorContainer = document.querySelector("div[data-sf-role='error-message-container']");

    if (errorContainer) {
        errorContainer.innerText = "";
        errorContainer.classList.add("d-none");
    }
};
//phoneinput.addEventListener('blur', function () {
//    returnvalue = PhoneBlurHandler(null, "/api/Validation/CheckNumberIsValid", phoneinput);
//    if (returnvalue) {
//        mobilenumberreview.innerHtml = 'Mobile number: ' + phoneinput.value;
//        pinreview.innerHTML = 'PIN: ' + rechargepin.value;
//        btnContinue.removeAttribute("disabled", "");
//    }
//    else {
//        btnContinue.setAttribute("disabled", "");
//    }
//});

function AppendError(err) {
    var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
    if (!errorMessageContainer) return;

    // errorMessageContainer.innerText += err + "\n";
    let existing = errorMessageContainer.innerText;
    if (!existing.includes(err)) {
        errorMessageContainer.innerText += err + "\n";
    }
   // errorMessageContainer.innerText = err;
    errorMessageContainer.classList.remove("d-none");
    errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
}

function showError(err) {
    var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
    if (!errorMessageContainer) return;

    errorMessageContainer.innerText = err;
    errorMessageContainer.classList.remove("d-none");
    errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
}

function validateFieldOnSubmit(input) {
    let isValid = true;
    if (input.hasAttribute('data-sf-role') && input.getAttribute('data-sf-role') === 'required' && !input.value) {
        invalidateElement(input);
        AppendError('* ' +input.dataset.label + '  is required.');
        isValid = false;
        return isValid;
    }

    if (input.name === 'mobile_number') {
        if (!input.value.match(/^\d{7}$/)) {
            invalidateElement(input);
            AppendError(' * The number entered is not a valid Vodafone number');
            isValid = false;
            return isValid;
        }
    }

    //hideElement(errorMessageContainer);
    return isValid;
}
async function validateForm() {
    resetValidationErrors(form);
    let isValid = true;
    var requiredInputs = document.querySelectorAll("input[data-sf-role='required']");

    requiredInputs.forEach(function (input) {
        if (!validateFieldOnSubmit(input)) {
            isValid = false;
        }
    });

    if (isValid) {
        resetValidationErrors(form);
        returnvalue = await PhoneBlurHandler(null, "/api/Validation/CheckNumberIsValid", phoneinput);
        if (returnvalue) {
            resetValidationErrors(form)
            mobilenumberreview.innerHtml = 'Mobile number: ' + phoneinput.value;
            pinreview.innerHTML = 'PIN: ' + rechargepin.value;
        }
        else {
            invalidateElement(phoneinput);
            AppendError('* The number entered is not a valid Vodafone number."');
            isValid = false;
        }
    }

    return isValid;
}

function getCookie(name) {
    let cookieArr = document.cookie.split(";");

    for (let i = 0; i < cookieArr.length; i++) {
        let cookiePair = cookieArr[i].split("=");

        // Removing whitespace at the beginning of the cookie name
        if (name == cookiePair[0].trim()) {
            // Decode the value to handle special characters
            return decodeURIComponent(cookiePair[1]);
        }
    }

    // Return null if not found
    return null;
}
async function getDecryptedCookie() {
    try {
        const response = await fetch('/api/Common/GetCookie', { method: "GET", credentials: "include" });
        if (!response.ok) return null;
        return await response.json();
    } catch (error) {
        console.error("Error fetching decrypted cookie:", error);
        return null;
    }
}