(function () {
  'use strict';
  if (localStorage.getItem("syntodarktheme")) {
      document.querySelector("html").setAttribute("data-theme-mode", "dark")
      document.querySelector("html").setAttribute("data-menu-styles", "dark")
      document.querySelector("html").setAttribute("data-header-styles", "dark")
  }
  if (localStorage.syntortl) {
      let html = document.querySelector('html');
      html.setAttribute("dir", "rtl");
      document.querySelector("#style")?.setAttribute("href", "../assets/libs/bootstrap/css/bootstrap.rtl.min.css");
  }
  if (localStorage.syntolayout) {
      let html = document.querySelector('html');
      html.setAttribute("data-nav-layout", "horizontal");
      document.querySelector("html").setAttribute("data-menu-styles", "light")
  }
  if (localStorage.getItem("syntolayout") == "horizontal") {
      document.querySelector("html").setAttribute("data-nav-layout", "horizontal")
  }
  if(localStorage.loaderEnable == 'true'){
      document.querySelector("html").setAttribute("loader","enable");
  }else{
      if(!document.querySelector("html").getAttribute("loader")){
          document.querySelector("html").setAttribute("loader","disable");
      }
  }

  function localStorageBackup() {

      // if there is a value stored, update color picker and background color
      // Used to retrive the data from local storage
      if (localStorage.primaryRGB) {
          if (document.querySelector('.theme-container-primary')) {
              document.querySelector('.theme-container-primary').value = localStorage.primaryRGB;
          }
          document.querySelector('html').style.setProperty('--primary-rgb', localStorage.primaryRGB);
      }
      if (localStorage.bodyBgRGB && localStorage.bodylightRGB) {
          if (document.querySelector('.theme-container-background')) {
              document.querySelector('.theme-container-background').value = localStorage.bodyBgRGB;
          }
          document.querySelector('html').style.setProperty('--body-bg-rgb', localStorage.bodyBgRGB);
          document.querySelector('html').style.setProperty('--body-bg-rgb2', localStorage.bodylightRGB);
          document.querySelector('html').style.setProperty('--light-rgb', localStorage.bodylightRGB);
          document.querySelector('html').style.setProperty('--form-control-bg', `rgb(${localStorage.bodylightRGB})`);
          document.querySelector('html').style.setProperty('--input-border', "rgba(255,255,255,0.1)");
          let html = document.querySelector('html');
          html.setAttribute('data-theme-mode', 'dark');
          html.setAttribute('data-menu-styles', 'dark');
          html.setAttribute('data-header-styles', 'dark');
      }
      if (localStorage.syntodarktheme) {
          let html = document.querySelector('html');
          html.setAttribute('data-theme-mode', 'dark');
      }
      if (localStorage.syntolayout) {
          let html = document.querySelector('html');
          let layoutValue = localStorage.getItem('syntolayout');
          html.setAttribute('data-nav-layout', 'horizontal');
          setTimeout(() => {
              clearNavDropdown();
          }, 5000);
          html.setAttribute('data-nav-style', 'menu-click');
          setTimeout(() => {
              checkHoriMenu();
          }, 5000);
      }
      if (localStorage.syntoverticalstyles) {
          let html = document.querySelector('html');
          let verticalStyles = localStorage.getItem('syntoverticalstyles');
          localStorage.removeItem("syntonavstyles")

          if (verticalStyles == 'default') {
              html.setAttribute('data-vertical-style', 'default');
              localStorage.removeItem("syntonavstyles")
          }
          if (verticalStyles == 'closed') {
              html.setAttribute('data-vertical-style', 'closed');
              localStorage.removeItem("syntonavstyles")
          }
          if (verticalStyles == 'icontext') {
              html.setAttribute('data-vertical-style', 'icontext');
              localStorage.removeItem("syntonavstyles")
          }
          if (verticalStyles == 'overlay') {
              html.setAttribute('data-vertical-style', 'overlay');
              localStorage.removeItem("syntonavstyles")
          }
          if (verticalStyles == 'detached') {
              html.setAttribute('data-vertical-style', 'detached');
              localStorage.removeItem("syntonavstyles")
          }
          if (verticalStyles == 'doublemenu') {
              html.setAttribute('data-vertical-style', 'doublemenu');
              localStorage.removeItem("syntonavstyles")
              setTimeout(() => {

                  const menuSlideItem = document.querySelectorAll(".main-menu > li > .side-menu__item");

                  // Create the tooltip element
                  const tooltip = document.createElement("div");
                  tooltip.className = "custome-tooltip";
                  // tooltip.textContent = "This is a tooltip";

                  // Set the CSS properties of the tooltip element
                  tooltip.style.setProperty("position", "fixed");
                  tooltip.style.setProperty("display", "none");
                  tooltip.style.setProperty("padding", "0.5rem");
                  tooltip.style.setProperty("white-space", "nowrap");
                  tooltip.style.setProperty("font-weight", "500");
                  tooltip.style.setProperty("font-size", "0.75rem");
                  tooltip.style.setProperty("background-color", "rgb(15, 23 ,42)");
                  tooltip.style.setProperty("color", "rgb(255, 255 ,255)");
                  tooltip.style.setProperty("margin-inline-start", "45px");
                  tooltip.style.setProperty("border-radius", "0.25rem");
                  tooltip.style.setProperty("z-index", "99");
                  console.log(menuSlideItem);
                 
                  menuSlideItem.forEach((e) => {
                    // Add an event listener to the menu slide item to show the tooltip
                    e.addEventListener("mouseenter", () => {
                      if (localStorage.syntoverticalstyles == "doublemenu") {
                        tooltip.style.setProperty("display", "block");
                        tooltip.textContent =
                          e.querySelector(".side-menu__label").textContent;
                        if (document.querySelector("html").getAttribute("data-vertical-style") == "doublemenu") {
                          e.appendChild(tooltip);
                        }
                      }
                    });
        
                    // Add an event listener to hide the tooltip
                    e.addEventListener("mouseleave", () => {
                      tooltip.style.setProperty("display", "none");
                      tooltip.textContent = e.querySelector(".side-menu__label").textContent;
                    });
                  });
              }, 1000);
          }
      }
      if (localStorage.syntonavstyles) {
          let html = document.querySelector('html');
          let navStyles = localStorage.getItem('syntonavstyles');
          if (navStyles == 'menu-click') {
              html.setAttribute('data-nav-style', 'menu-click');
              localStorage.removeItem("syntoverticalstyles");
              html.removeAttribute('data-vertical-style');
          }
          if (navStyles == 'menu-hover') {
              html.setAttribute('data-nav-style', 'menu-hover');
              localStorage.removeItem("syntoverticalstyles");
              html.removeAttribute('data-vertical-style');
              setTimeout(() => {
                  menuhoverFn();
              }, 1000);
          }
          if (navStyles == 'icon-click') {
              html.setAttribute('data-nav-style', 'icon-click');
              localStorage.removeItem("syntoverticalstyles");
              html.removeAttribute('data-vertical-style');
          }
          if (navStyles == 'icon-hover') {
              html.setAttribute('data-nav-style', 'icon-hover');
              localStorage.removeItem("syntoverticalstyles");
              html.removeAttribute('data-vertical-style');
          }
      }
      if (localStorage.syntoclassic) {
          let html = document.querySelector('html');
          html.setAttribute('data-page-style', 'classic');
      }
      if (localStorage.syntomodern) {
          let html = document.querySelector('html');
          html.setAttribute('data-page-style', 'modern');
      }
      if (localStorage.syntoboxed) {
          let html = document.querySelector('html');
          html.setAttribute('data-width', 'boxed');
      }
      if (localStorage.syntoheaderfixed) {
          let html = document.querySelector('html');
          html.setAttribute('data-header-position', 'fixed');
      }
      if (localStorage.syntoheaderscrollable) {
          let html = document.querySelector('html');
          html.setAttribute('data-header-position', 'scrollable');
      }
      if (localStorage.syntomenufixed) {
          let html = document.querySelector('html');
          html.setAttribute('data-menu-position', 'fixed');
      }
      if (localStorage.syntomenuscrollable) {
          let html = document.querySelector('html');
          html.setAttribute('data-menu-position', 'scrollable');
      }
      if (localStorage.syntoMenu) {
          let html = document.querySelector('html');
          let menuValue = localStorage.getItem('syntoMenu');
          switch (menuValue) {
              case 'light':
                  html.setAttribute('data-menu-styles', 'light');
                  break;
              case 'dark':
                  html.setAttribute('data-menu-styles', 'dark');
                  break;
              case 'color':
                  html.setAttribute('data-menu-styles', 'color');
                  break;
              case 'gradient':
                  html.setAttribute('data-menu-styles', 'gradient');
                  break;
              case 'transparent':
                  html.setAttribute('data-menu-styles', 'transparent');
                  break;
              default:
                  break;
          }
      }
      if (localStorage.syntoHeader) {
          let html = document.querySelector('html');
          let headerValue = localStorage.getItem('syntoHeader');
          switch (headerValue) {
              case 'light':
                  html.setAttribute('data-header-styles', 'light');
                  break;
              case 'dark':
                  html.setAttribute('data-header-styles', 'dark');
                  break;
              case 'color':
                  html.setAttribute('data-header-styles', 'color');
                  break;
              case 'gradient':
                  html.setAttribute('data-header-styles', 'gradient');
                  break;
              case 'transparent':
                  html.setAttribute('data-header-styles', 'transparent');
                  break;

              default:
                  break;
          }
      }
      if (localStorage.bgimg) {
          let html = document.querySelector('html');
          let value = localStorage.getItem('bgimg');
          html.setAttribute('data-bg-img', value);
      }
  }
  localStorageBackup()

})();

document.addEventListener('DOMContentLoaded', function () {
    const radioButtons = document.querySelectorAll('input[name="WebTopUp_value"]');
    const display = document.getElementById('selection_display');
    const totalDisplay = document.getElementById('total_display');

    function updateDisplays() {
        const selected = document.querySelector('input[name="WebTopUp_value"]:checked');
        const value = selected ? Number(selected.value) : 0;
        if (display != null) {
            display.textContent = `$${value}`;
            totalDisplay.textContent = `$${value}`;
        }
    }

    radioButtons.forEach(radio => {
        radio.addEventListener('change', updateDisplays);
    });

    updateDisplays();
    (function () {
        // Your existing loader functions/elements from before
        const loader = document.getElementById('loader');

        // FETCH INTERCEPTOR (works with window.fetch)
        let fetchCount = 0;
        const originalFetch = window.fetch;
        window.fetch = async function (...args) {
            fetchCount++;
            if (fetchCount === 1) loader.classList.remove("d-none");
            try {
                return await originalFetch.apply(this, args);
            } finally {
                fetchCount--;
                if (fetchCount === 0) loader.classList.add("d-none");
            }
        };

        // XHR INTERCEPTOR (covers XMLHttpRequest)
        let xhrCount = 0;
        const originalOpen = XMLHttpRequest.prototype.open;
        XMLHttpRequest.prototype.open = function () {
            xhrCount++;
            if (xhrCount === 1) loader.style.display = 'flex';
            originalOpen.apply(this, arguments);
        };

        const originalSend = XMLHttpRequest.prototype.send;
        XMLHttpRequest.prototype.send = function () {
            this.addEventListener('loadend', () => {
                xhrCount--;
                if (xhrCount === 0 && fetchCount === 0) loader.style.display = 'none';
            });
            originalSend.apply(this, arguments);
        };
    })();
});



function enableNavigationLoader() {
    const loader = document.getElementById("loader");
    if (!loader) return;
    const showLoader = () => loader.classList.remove("d-none");
    const hideLoader = () => loader.classList.add("d-none");

    // 1. Anchor clicks
    document.addEventListener("click", function (e) {
        const link = e.target.closest("a");
        if (!link) return;
        const href = link.getAttribute("href");
        if (href && href !== "#" && !href.startsWith("javascript") && !link.hasAttribute("target")) {
            showLoader();
        }
    }); 
    // 2. Form submit
    document.addEventListener("submit", function () {
        showLoader();
    }); 


    window.addEventListener("pageshow", function () {
        hideLoader();
    });

    // 4. pushState / replaceState
    const originalPushState = history.pushState;
    history.pushState = function () {
        showLoader();
        return originalPushState.apply(this, arguments);
    };
    const originalReplaceState = history.replaceState;
    history.replaceState = function () {
        showLoader();
        return originalReplaceState.apply(this, arguments);
    };

    // 5. Page refresh / redirect
    window.addEventListener("beforeunload", showLoader);
}
enableNavigationLoader();    
       
           

function openFullscreen() {
    const elem = document.documentElement;
    const openIcon = document.querySelector('.full-screen-open');
    const closeIcon = document.querySelector('.full-screen-close');

    if (!document.fullscreenElement) {
        if (elem.requestFullscreen) {
            elem.requestFullscreen();
        } else if (elem.webkitRequestFullscreen) { // Safari
            elem.webkitRequestFullscreen();
        } else if (elem.msRequestFullscreen) { // IE11
            elem.msRequestFullscreen();
        }
        if (openIcon) openIcon.classList.add('d-none');
        if (closeIcon) closeIcon.classList.remove('d-none');
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.webkitExitFullscreen) { // Safari
            document.webkitExitFullscreen();
        } else if (document.msExitFullscreen) { // IE11
            document.msExitFullscreen();
        }
        if (openIcon) openIcon.classList.remove('d-none');
        if (closeIcon) closeIcon.classList.add('d-none');
    }
}

// Listen for fullscreen changes (ESC/F11)
document.addEventListener('fullscreenchange', function() {
    const openIcon = document.querySelector('.full-screen-open');
    const closeIcon = document.querySelector('.full-screen-close');
    if (!document.fullscreenElement) {
        if (openIcon) openIcon.classList.remove('d-none');
        if (closeIcon) closeIcon.classList.add('d-none');
    } else {
        if (openIcon) openIcon.classList.add('d-none');
        if (closeIcon) closeIcon.classList.remove('d-none');
    }
});

 document.addEventListener('DOMContentLoaded', function () {
     const toggle = document.querySelector('.header-theme-mode .layout-setting');
     const fullscreenaction = document.getElementById('fullscreenaction');
    if (!toggle) return;

    toggle.addEventListener('click', function () {
        const html = document.documentElement;
        const currentTheme = html.getAttribute('data-theme-mode') || 'light';
        const nextTheme = currentTheme === 'light' ? 'dark' : 'light';

        html.setAttribute('data-theme-mode', nextTheme);
        html.setAttribute('data-header-styles', nextTheme);

        if (nextTheme === 'dark') {
            localStorage.setItem('syntodarktheme', 'true');
        } else {
            localStorage.removeItem('syntodarktheme');
        }
    });

    // On load, restore theme from syntodarktheme
    if (localStorage.getItem('syntodarktheme')) {
        document.documentElement.setAttribute('data-theme-mode', 'dark');
        document.documentElement.setAttribute('data-header-styles', 'dark');
    } else {
        document.documentElement.setAttribute('data-theme-mode', 'light');
        document.documentElement.setAttribute('data-header-styles', 'light');
     }

     fullscreenaction.addEventListener('click', function () {
         openFullscreen();
     });
 });


