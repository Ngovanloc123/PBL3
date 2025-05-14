
// ================== SLIDER BOOKS ========================
// ================== SLIDER BOOKS ========================
new Swiper('.card-wrapper', {
    loop: true,
    spaceBetween: 32,

    pagination: {
        el: '.swiper-pagination',
    },

    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },

    autoplay: {
        delay: 4000, // 4 giây
        disableOnInteraction: false // vẫn tự động sau khi người dùng tương tác
    },

    breakpoints: {
        0: {
            slidesPerView: 2
        },
        768: {
            slidesPerView: 3
        },
        1024: {
            slidesPerView: 4
        },
    }
});


/*============================= BOOK LIST =============================*/

const initSlider = (containerSelector) => {
    const container = document.querySelector(containerSelector);
    if (!container) return; // Kiểm tra nếu container không tồn tại

    const bookList = container.querySelector(".book-list");
    const slideButtons = container.querySelectorAll(".card-button");

    slideButtons.forEach(button => {
        button.addEventListener("click", () => {
            const direction = button.id === "prev-card" ? -1 : 1;
            const scrollAmount = bookList.clientWidth * direction;

            bookList.scrollBy({
                left: scrollAmount,
                behavior: "smooth"
            });
        });
    });
};

// Khởi tạo cho nhiều danh sách khác nhau
document.addEventListener("DOMContentLoaded", () => {
    initSlider(".category1");
    initSlider(".category2");
    initSlider(".category3");
    initSlider(".category4");
    initSlider(".category5");
    initSlider(".category6");
    initSlider(".category7");
    initSlider(".category8");
});

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============

// =============== Literature & Fiction ===============



