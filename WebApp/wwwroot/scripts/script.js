$(document).ready(function () {

    // Update active title button on slide change
    const carousel = document.querySelector('#carouselExample');
    const titleButtons = document.querySelectorAll('.carousel-titles button');
    if (carousel != null) {
        carousel.addEventListener('slid.bs.carousel', function (event) {
            titleButtons.forEach(btn => btn.classList.remove('active'));
            titleButtons[event.to].classList.add('active');
        });
    }

    // Make title buttons clickable (like tabs)
    if (titleButtons != null && titleButtons.length > 0) {
        titleButtons.forEach((btn, index) => {
            btn.addEventListener('click', () => {
                const bsCarousel = bootstrap.Carousel.getInstance(carousel);
                bsCarousel.to(index);
            });
        });
    }
    //const menutoggle = document.getElementById('menu-toggle');
    const menu = document.getElementById('menu');
    const menuItems = menu.querySelectorAll('li');
    const overlay = document.querySelector('.mega-overlay');

    //// Toggle mobile menu
    //menutoggle.addEventListener('click', () => {
    //    menu.classList.toggle('show');
    //});

    // Mobile accordion for mega menu
    if (menu != null) {
        menuItems.forEach(item => {
            item.addEventListener('click', (e) => {
                if (window.innerWidth <= 992) {
                    const mega = item.querySelector(".mega-menu");
                    if (mega) {
                        e.preventDefault();

                        // Close other open menus
                        menuItems.forEach(otherItem => {
                            if (otherItem !== item) {
                                otherItem.classList.remove('active');
                            }
                        });

                        // Toggle current menu
                        item.classList.toggle('active');
                    }
                }
            });
        });
    }
    if (overlay != null) {
        // Close mobile menu on overlay click
        overlay.addEventListener('click', () => {
            menu.classList.remove('show');
            menuItems.forEach(item => item.classList.remove('active'));
        });
    }

  /*------custom-cursor-----*/
  if ($(".custom-cursor").length) {
		var cursor = document.querySelector(".custom-cursor-one");
		var cursorinner = document.querySelector(".custom-cursor-two");
		var a = document.querySelectorAll("a");

		document.addEventListener("mousemove", function (e) {
			var x = e.clientX;
			var y = e.clientY;
			cursor.style.transform = `translate3d(calc(${e.clientX}px - 50%), calc(${e.clientY}px - 50%), 0)`;
		});

		document.addEventListener("mousemove", function (e) {
			var x = e.clientX;
			var y = e.clientY;
			cursorinner.style.left = x + "px";
			cursorinner.style.top = y + "px";
		});

		document.addEventListener("mousedown", function () {
			cursor.classList.add("click");
			cursorinner.classList.add("custom-cursor-innerhover");
		});

		document.addEventListener("mouseup", function () {
			cursor.classList.remove("click");
			cursorinner.classList.remove("custom-cursor-innerhover");
		});

		a.forEach((item) => {
			item.addEventListener("mouseover", () => {
				cursor.classList.add("custom-cursor-hover");
			});
			item.addEventListener("mouseleave", () => {
				cursor.classList.remove("custom-cursor-hover");
			});
		});
	}
  //------ testimonial-one-tabs
  $('.testimonial-one-tabs .owl-carousel').owlCarousel({
    items: 1,
    loop: true,
    startPosition: 'URLHash'
  });
  $('.testimonial-one-btn li').on('click', function () {
    $('.testimonial-one-btn li').removeClass('active');
    $(this).addClass('active');
  })
 

  // -----testimonial-three-tabs
  $('.testimonial-three-tabs .owl-carousel').owlCarousel({
    items: 1,
    loop: true,
    margin: 20,
    // autoplay: true,
    // autoplayTimeout: 5000,
    startPosition: 'URLHash'
  });
  $('.testimonial-three-btn li').on('click', function () {
    $('.testimonial-three-btn li').removeClass('active');
    $(this).addClass('active');
  })

  // -----eshop-product-slider
  $('.eshop-product-slider').owlCarousel({
    items: 4,
    loop: true,
    margin: 30,
    autoplay: true,
    autoplayTimeout: 155000,
    responsive: {
      0: {
        items: 1,
      },
      768: {
        items: 2,
      },
      992: {
        items: 3,
      },
      1199: {
        items: 4,
      }
    }
  })

  // -----banner-four-bottom
  $('.banner-four-bottom .owl-carousel').owlCarousel({
    items: 9,
    loop: true,
    margin: 30,
    dots: false,
    autoplay: true,
    autoplayTimeout: 3000,
    slideTransition: 'linear',
    smartSpeed: 3000,
    responsive: {
      0: {
        items: 5,
      },
      768: {
        items: 5,
      },
      992: {
        items: 6,
      },
      1199: {
        items: 7,
      },
      1400: {
        items: 9,
      }
    }
  })

  // -----package-four
  $('.package-four-slider').owlCarousel({
    items: 4,
    loop: true,
    dots: true,
    autoplay: true,
    autoplayTimeout: 10000,
    slideTransition: 'linear',
    smartSpeed: 500,
    responsive: {
      0: {
        autoHeight: true,
        items: 1,
      },
      768: {
        items: 1,
      },
      992: {
        items: 4,
      },
      1199: {
        items: 4,
      }
    }
  })

  // -----featured-movies-two-slider
  $('.featured-movies-two-slider').owlCarousel({
    items: 1,
    loop: true,
    dots: false,
    margin: 30,
    nav: true,
    // animateOut: 'fadeOut',
    autoplay: true,
    autoplayTimeout: 5000,
    navText: [
      '<i class="icon-left-arrow-two" aria-hidden="true"></i>',
      '<i class="icon-right-arrow-two" aria-hidden="true"></i>'
    ],
  })

  // -----movies-six-slider
  $('.movies-six-slider, .comingsoon-popular-movie-six-slider').owlCarousel({
    items: 4,
    loop: true,
    dots: false,
    nav: true,
    // animateOut: 'fadeOut',
    autoplay: true,
    margin: 24,
    autoplayTimeout: 5000,
    navText: [
      '<i class="fas fa-arrow-left" aria-hidden="true"></i>',
      '<i class="fas fa-arrow-right" aria-hidden="true"></i>'
    ],
    responsive: {
      0: {
        autoHeight: true,
        items: 1,
      },
      768: {
        items: 2,
      },
      992: {
        items: 3,
      },
      1199: {
        items: 4,
      }
    }
  })

  // -----blog-four-slider
  $('.blog-four-slider').owlCarousel({
    items: 2,
    loop: true,
    dots: false,
    nav: true,
    animateOut: 'fadeOut',
    autoplay: true,
    autoplayTimeout: 5000,
    responsive: {
      0: {
        autoHeight: true,
        items: 1,
      },
      768: {
        items: 2,
      },
      992: {
        items: 2,
      },
      1199: {
        items: 2,
      }
    },
    navText: [
      '<i class="fas fa-arrow-left" aria-hidden="true"></i>',
      '<i class="fas fa-arrow-right" aria-hidden="true"></i>'
    ],

  })

  // -----testimonial-four-slider
  $('.testimonial-four .owl-carousel, .testimonial-three-tabs .owl-carousel, .testimonial-five .owl-carousel, .testimonial-six .owl-carousel').owlCarousel({
    items: 3,
    loop: true,
    // autoplay: true,
    // autoplayTimeout: 10000,
    // smartSpeed: 2000,
    responsive: {
      0: {
        autoHeight: true,
        items: 1,
      },
      576: {
        autoHeight: true,
        items: 1,
      },
      768: {
        items: 2,
      },
      992: {
        items: 2,
      },
      1199: {
        autoHeight: true,
        items: 3,
      }
    }
  });

  // -----popular-movie-video-six-slider
  $('.popular-movie-video-six-box').owlCarousel({
    items: 3,
    loop: true,
    margin: 24,
    dots: false,
    // autoplay: true,
    // autoplayTimeout: 3000,
    slideTransition: 'linear',
    nav: true,
    responsive: {
      0: {
        items: 1,
      },
      768: {
        items: 1,
      },
      992: {
        items: 2,
      },
      1199: {
        items: 3,
      },
      1400: {
        items: 3,
      }
    },
    navText: [
      '<i class="fas fa-arrow-left" aria-hidden="true"></i>',
      '<i class="fas fa-arrow-right" aria-hidden="true"></i>'
    ],
  })

  if ($(".wow").length) {
		var wow = new WOW({
			boxClass: "wow", // animated element css class (default is wow)
			animateClass: "animated", // animation css class (default is animated)
			mobile: true, // trigger animations on mobile devices (default is true)
			live: true // act on asynchronously loaded content (default is true)
		});
		wow.init();
	}

    $('body').on('click', 'a[href="#"]', function (e) { e.preventDefault() });

    /*------search-toggler-----*/
    
        $(".search-toggler").on("click", function (e) {
            e.preventDefault();
            $(".search-popup").toggleClass("active");
            $("body").toggleClass("locked");
        });
    

  /*-----scroll-to-top------*/
  $(window).scroll(function () {
    if ($(this).scrollTop() > 100) {
      $('#scroll').fadeIn();
    } else {
      $('#scroll').fadeOut();
    }
  });
  $('#scroll').click(function () {
    $("html, body").animate({ scrollTop: 0 }, 1000);
    return false;
  });

  /*------sticky-header-----*/
  $(window).scroll(function () {
    if ($(this).scrollTop() > 200) {
      $('.main-header, .main-header-two, .main-header-four').addClass("sticky");
    }
    else {
      $('.main-header, .main-header-two, .main-header-four').removeClass("sticky");
    }
  });



  /*------mobile-nav-----*/
  jQuery(function ($) {
    $('.header-right-end, .header-two-right-end').click(function () {
      $('.mobile-nav-wrapper, .mobile-nav-two-wrapper').toggleClass('expanded')
      $("body").toggleClass("locked");
    })
  })
  if ($(".mobile-nav-toggler, .mobile-nav-two-toggler").length) {
    $(".mobile-nav-toggler, .mobile-nav-two-toggler").on("click", function (e) {
      e.preventDefault();
      $(".mobile-nav-wrapper, .mobile-nav-two-wrapper").toggleClass("expanded");
      $("body").toggleClass("locked");
    });
  }
  if ($(".mobile-nav-container .main-menu-list, .mobile-nav-two-container .main-menu-list").length) {
    let dropdownAnchor = $(
      ".mobile-nav-container .main-menu-list .dropdown > a, .mobile-nav-two-container .main-menu-list .dropdown > a"
    );
    dropdownAnchor.each(function () {
      let self = $(this);
      let toggleBtn = document.createElement("BUTTON");
      toggleBtn.setAttribute("aria-label", "dropdown toggler");
      toggleBtn.innerHTML = "<i class='fa fa-angle-down'></i>";
      self.append(function () {
        return toggleBtn;
      });
      self.find("button").on("click", function (e) {
        e.preventDefault();
        let self = $(this);
        self.toggleClass("expanded");
        self.parent().toggleClass("expanded");
        self.parent().parent().children("ul").slideToggle();
      });
    });
  }

  /*------locked-----*/
  jQuery(function ($) {
    $('.side-drawer-toggler-btn').click(function () {
      $('.side-drawer-wrapper').toggleClass('expanded')
      $("body").toggleClass("locked");
    })
  })
  if ($(".side-drawer-toggler").length) {
    $(".side-drawer-toggler").on("click", function (e) {
      e.preventDefault();
      $(".side-drawer-wrapper").toggleClass("expanded");
      $("body").toggleClass("locked");
    });
  }


 

  /*------current-----*/
  function dynamicCurrentMenuClass(selector) {
    let FileName = window.location.href.split("/").reverse()[0];
    selector.find("li").each(function () {
      let anchor = $(this).find("a");
      if ($(anchor).attr("href") == FileName) {
        $(this).addClass("current");
      }
    });
    // if any li has .current elmnt add class
    selector.children("li").each(function () {
      if ($(this).find(".current").length) {
        $(this).addClass("current");
      }
    });
    // if no file name return
    if ("" == FileName) {
      selector.find("li").eq(0).addClass("current");
    }
  }
  if ($(".navbar-nav, .navbar-nav-two").length) {
    // dynamic current class
    let mainNavUL = $(".navbar-nav, .navbar-nav-two");
    dynamicCurrentMenuClass(mainNavUL);
  }
  if ($(".service-details__sidebar-service-list").length) {
    // dynamic current class
    let mainNavUL = $(".service-details__sidebar-service-list");
  }

  // ---------------video-popup
  if ($(".video-popup").length) {
		$('.video-popup').YouTubePopUp();
	}

  // ---------------faq-count
  $('.faq-count').each(function () {
    $(this).prop('Counter', 0).animate({
      Counter: $(this).text()
    }, {
      duration: 9000,
      easing: 'swing',
      step: function (now) {
        $(this).text(Math.ceil(now));
      }
    });
    $('.faq-count').addClass('animated fadeInLeft');
  });

  // ----product-details  Add-to-Cart
  var buttonPlus = $(".qty-btn-plus");
  var buttonMinus = $(".qty-btn-minus");
  var incrementPlus = buttonPlus.click(function () {
    var $n = $(this)
      .parent(".qty-container")
      .find(".input-qty");
    $n.val(Number($n.val()) + 1);
  });
  var incrementMinus = buttonMinus.click(function () {
    var $n = $(this)
      .parent(".qty-container")
      .find(".input-qty");
    var amount = Number($n.val());
    if (amount > 0) {
      $n.val(amount - 1);
    }
  });

  if ($('.testimonial-thumb').length) {
		var review_thumb = new Swiper(".testimonial-thumb",{
			slidesPerView: 3,
			spaceBetween: 0,
		})
	}
	if ($('.testimonial-reviews').length) {
		var review_swiper = new Swiper(".testimonial-reviews",{
			slidesPerView:1,
			loop:true,
			spaceBetween: 0,
			autoplay: {
				delay: 2500,
				disableOnInteraction: false,
			},
			pagination: {
				el: ".swiper-pagination",
				clickable: true,
			},
			thumbs: {
				swiper: review_thumb,
			},
		})
	}


});



// Window load

$(window).on("load", function () {
  /*------preloader-----*/
  if ($(".preloader").length) {
    $(".preloader").fadeOut();
  }

});




//class FileUploadComponent {
//    constructor() {
//        this.uploadBox = document.getElementById('uploadBox');
//        this.fileInput = document.getElementById('fileInput');
//        this.filesPreview = document.getElementById('filesPreview');
//        this.filesList = document.getElementById('filesList');
//        this.uploadProgress = document.getElementById('uploadProgress');
//        this.uploadComplete = document.getElementById('uploadComplete');
//        this.addMoreBtn = document.getElementById('addMoreBtn');
//        this.newUploadBtn = document.getElementById('newUploadBtn');
//        this.viewFilesBtn = document.getElementById('viewFilesBtn');

//        this.files = [];
//        this.maxFileSize = 10 * 1024 * 1024; // 10MB
//        this.allowedTypes = ['application/pdf', 'image/jpeg', 'image/png', 'image/gif'];

//        this.init();
//    }

//    init() {
//        this.setupEventListeners();
//    }

//    setupEventListeners() {
//        // Upload box events
//        this.uploadBox.addEventListener('click', () => {
//            this.fileInput.click();
//        });

//        this.fileInput.addEventListener('change', (e) => {
//            this.handleFiles(e.target.files);
//        });

//        // Drag and drop events
//        this.uploadBox.addEventListener('dragover', (e) => {
//            e.preventDefault();
//            this.uploadBox.classList.add('dragover');
//        });

//        this.uploadBox.addEventListener('dragleave', (e) => {
//            e.preventDefault();
//            this.uploadBox.classList.remove('dragover');
//        });

//        this.uploadBox.addEventListener('drop', (e) => {
//            e.preventDefault();
//            this.uploadBox.classList.remove('dragover');
//            this.handleFiles(e.dataTransfer.files);
//        });

//        // Action buttons
//        this.addMoreBtn.addEventListener('click', () => {
//            this.fileInput.click();
//        });

//        this.newUploadBtn.addEventListener('click', () => {
//            this.startNewUpload();
//        });

//        this.viewFilesBtn.addEventListener('click', () => {
//            this.viewUploadedFiles();
//        });

//        // Prevent default drag behaviors
//        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
//            document.addEventListener(eventName, (e) => {
//                e.preventDefault();
//                e.stopPropagation();
//            });
//        });
//    } handleFiles(fileList) {
//        const newFiles = Array.from(fileList);

//        newFiles.forEach(file => {
//            if (this.validateFile(file)) {
//                this.addFile(file);
//            }
//        });

//        if (this.files.length > 0) {
//            this.showPreview();
//            this.simulateUpload();
//        }
//    }

//    validateFile(file) {
//        // Check file type
//        if (!this.allowedTypes.includes(file.type)) {
//            this.showError(`${file.name}: Only PDF, JPG, PNG, and GIF files are allowed.`);
//            return false;
//        }

//        // Check file size
//        if (file.size > this.maxFileSize) {
//            this.showError(`${file.name}: File size must be less than 10MB.`);
//            return false;
//        }

//        // Check if file already exists
//        if (this.files.some(f => f.name === file.name && f.size === file.size)) {
//            this.showError(`${file.name}: File already selected.`);
//            return false;
//        }

//        return true;
//    }

//    addFile(file) {
//        const fileObj = {
//            file: file,
//            id: Date.now() + Math.random(),
//            name: file.name,
//            size: this.formatFileSize(file.size),
//            status: 'pending',
//            progress: 0
//        };

//        this.files.push(fileObj);
//        this.renderFile(fileObj);
//    }

//    renderFile(fileObj) {
//        const fileElement = document.createElement('div');
//        fileElement.className = 'file-item';
//        fileElement.setAttribute('data-file-id', fileObj.id);

//        // Create preview image
//        const reader = new FileReader();
//        reader.onload = (e) => {
//            fileElement.innerHTML = `
//                <img src="${e.target.result}" alt="${fileObj.name}" class="file-preview">
//                <div class="file-info">
//                    <div class="file-name">${fileObj.name}</div>
//                    <div class="file-size">${fileObj.size}</div>
//                </div>
//                <div class="file-status">
//                    <div class="status-icon status-uploading">⏳</div>
//                </div>
//                <div class="file-actions">
//                    <button class="file-action delete" onclick="fileUpload.removeFile('${fileObj.id}')">
//                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
//                            <path d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6M8 6V4a2 2 0 012-2h4a2 2 0 012 2v2"/>
//                        </svg>
//                    </button>
//                </div>
//            `;
//        };
//        reader.readAsDataURL(fileObj.file);

//        this.filesList.appendChild(fileElement);
//    }

//    showPreview() {
//        this.filesPreview.classList.add('show');
//        this.addMoreBtn.style.display = 'inline-block';
//    }

//    simulateUpload() {
//        this.uploadBox.classList.add('uploading');

//        let completedFiles = 0;
//        const totalFiles = this.files.length;

//        this.files.forEach((fileObj, index) => {
//            setTimeout(() => {
//                this.uploadFile(fileObj, (progress) => {
//                    const overallProgress = ((completedFiles + progress / 100) / totalFiles) * 100;
//                    this.updateProgress(overallProgress);

//                    if (progress === 100) {
//                        completedFiles++;
//                        if (completedFiles === totalFiles) {
//                            this.completeUpload();
//                        }
//                    }
//                });
//            }, index * 200);
//        });
//    }

//    uploadFile(fileObj, progressCallback) {
//        let progress = 0;
//        const fileElement = document.querySelector(`[data-file-id="${fileObj.id}"]`);

//        const uploadInterval = setInterval(() => {
//            progress += Math.random() * 15;
//            if (progress >= 100) {
//                progress = 100;
//                clearInterval(uploadInterval);

//                fileObj.status = 'success';
//                const statusIcon = fileElement.querySelector('.status-icon');
//                statusIcon.className = 'status-icon status-success';
//                statusIcon.textContent = '✓';
//            }

//            progressCallback(progress);
//        }, 100 + Math.random() * 200);
//    }

//    updateProgress(progress) {
//        const progressBar = document.querySelector('.progress-bar');
//        const progressText = document.querySelector('.progress-text');

//        const circumference = 2 * Math.PI * 25;
//        const offset = circumference - (progress / 100) * circumference;

//        progressBar.style.strokeDashoffset = offset;
//        progressText.textContent = Math.round(progress) + '%';
//    } completeUpload() {
//        setTimeout(() => {
//            this.uploadBox.style.display = 'none';
//            this.uploadComplete.style.display = 'block';

//            const completeTitle = this.uploadComplete.querySelector('.complete-title');
//            const completeSubtitle = this.uploadComplete.querySelector('.complete-subtitle');

//            completeTitle.textContent = 'Upload Successful!';
//            completeSubtitle.textContent = `${this.files.length} file(s) uploaded successfully`;
//        }, 500);
//    }

//    startNewUpload() {
//        // Reset all states
//        this.files = [];

//        // Clear files list
//        this.filesList.innerHTML = '';

//        // Reset displays
//        this.uploadComplete.style.display = 'none';
//        this.uploadBox.style.display = 'block';
//        this.uploadBox.classList.remove('uploading', 'success');
//        this.filesPreview.classList.remove('show');
//        this.addMoreBtn.style.display = 'none';

//        // Reset progress
//        const progressBar = document.querySelector('.progress-bar');
//        const progressText = document.querySelector('.progress-text');
//        progressBar.style.strokeDashoffset = '157';
//        progressText.textContent = '0%';

//        // Re-setup file input
//        this.fileInput.value = '';
//    }

//    viewUploadedFiles() {
//        // Show the uploaded files by hiding complete screen and showing files
//        this.uploadComplete.style.display = 'none';
//        this.filesPreview.classList.add('show');
//        this.addMoreBtn.style.display = 'inline-block';

//        // Update preview title
//        const previewTitle = this.filesPreview.querySelector('.preview-title');
//        previewTitle.textContent = 'Uploaded Files';
//    }

//    formatFileSize(bytes) {
//        if (bytes === 0) return '0 Bytes';

//        const k = 1024;
//        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
//        const i = Math.floor(Math.log(bytes) / Math.log(k));

//        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
//    }

//    showError(message) {
//        const errorDiv = document.createElement('div');
//        errorDiv.className = 'error-notification';
//        errorDiv.style.animation = 'slideInRight 0.3s ease';
//        errorDiv.textContent = message;

//        document.body.appendChild(errorDiv);

//        setTimeout(() => {
//            errorDiv.style.animation = 'slideOutRight 0.3s ease';
//            setTimeout(() => errorDiv.remove(), 300);
//        }, 4000);
//    }

//    removeFile(fileId) {
//        this.files = this.files.filter(f => f.id != fileId);
//        const fileElement = document.querySelector(`[data-file-id="${fileId}"]`);

//        if (fileElement) {
//            fileElement.style.animation = 'slideOut 0.3s ease forwards';
//            setTimeout(() => {
//                fileElement.remove();

//                if (this.files.length === 0) {
//                    this.filesPreview.classList.remove('show');
//                    this.addMoreBtn.style.display = 'none';
//                    this.uploadBox.classList.remove('uploading');
//                }
//            }, 300);
//        }
//    }
//}

// Add slide animations for notifications
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    @keyframes slideOut {
        from {
            opacity: 1;
            transform: translateY(0);
        }
        to {
            opacity: 0;
            transform: translateY(-20px);
        }
    }
`;
document.head.appendChild(style);

//// Initialize the component
//let fileUpload;
//document.addEventListener('DOMContentLoaded', () => {
//    fileUpload = new FileUploadComponent();
//});

//// Export for potential module use
//if (typeof module !== 'undefined' && module.exports) {
//    module.exports = FileUploadComponent;
//}



  // -----pricing-package-slider
  $('.pricing-package-slider').owlCarousel({
    items: 3,
    loop: true,
    dots: true,
    margin: 30,
    autoplay: true,
    autoplayTimeout: 5000,
    slideTransition: 'linear',
    smartSpeed: 500,
    responsive: {
      0: {
        autoHeight: true,
        items: 1,
        margin: 15,
      },
      768: {
        items: 2,
        margin: 20,
      },
      992: {
        items: 3,
        margin: 30,
      },
      1199: {
        items: 4,
        margin: 30,
      }
    }
  })

document.addEventListener("DOMContentLoaded", function () {
    const userDropdown = document.querySelector(".vdf-user-dropdown");
    if (userDropdown) {
        const toggle = userDropdown.querySelector(".dropdown-toggle");

        // Click toggle
        toggle.addEventListener("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            userDropdown.classList.toggle("is-open");
        });

        // Close on click outside
        document.addEventListener("click", function () {
            userDropdown.classList.remove("is-open");
        });
    }
    
});