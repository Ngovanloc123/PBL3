
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

    breakpoints: {
        0: {
            slidesPerView: 1
        },
        768: {
            slidesPerView: 2
        },
        1024: {
            slidesPerView: 3
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

//const initSlider = () => {
//    const listCard = document.querySelector(".Literature-Fiction .Literature-Fiction");

//    const slideButtons = document.querySelectorAll(".Literature-Fiction .card-button");
//    //console.log(slideButtons);

//    // Gán sự kiện cho từng nút
//    slideButtons.forEach(button => {
//        button.addEventListener("click", () => {
//            const direction = button.id === "prev-card" ? -1 : 1;
//            const scrollAmount = listCard.clientWidth * direction;

//            // Thực hiện cuộn danh sách
//            listCard.scrollBy({
//                left: scrollAmount,
//                behavior: "smooth"
//            });
//        });
//    });
//};


// BEGIN: Thêm Author vào danh sách author =))
function addAuthor() {
    // Lấy phần tử cần thêm
    let selectBox = document.getElementById("new-author");
    // Lấy giá trị cần thêm, vì value và text đều là AuthorName nên không cần lấy text
    let selectValue = selectBox.value;

    console.log(selectValue);

    if (selectValue != "--Select Author--") {


        let authorContainer = document.getElementById("author-container");
        // Vị trí cần thêm
        let index = document.querySelectorAll(".author-item").length;

        let newDiv = document.createElement("div");

        newDiv.classList.add("mb-3", "d-flex", "flex-sm-row", "author-item");
        newDiv.innerHTML = `
                <input value="${selectValue}" disabled class="form-control" />
                <input type="hidden" name="AuthorsName[${index}]" value="${selectValue}" />
				
				<button onclick="removeAuthor(this)" type="button" class="btn btn-secondary mx-2">
                    <i class="fa-solid fa-delete-left "></i>
                </button>
			`;

        // console.log(newDiv);

        authorContainer.appendChild(newDiv);

        console.log(authorContainer);

        // Reset dropdown về trạng thái mặc định
        selectBox.selectedIndex = 0;
    }
}
// END: Thêm Author vào danh sách author =))

// BEGIN: Xoá author vào danh sách author =))
function removeAuthor(button) {
    button.parentElement.remove();

    // Cập nhật lại index cho tất cả select còn lại
    let selects = document.querySelectorAll('.author-item select');
    selects.forEach((select, index) => {
        select.setAttribute("name", `AuthorsName[${index}]`);

        console.log(authorContainer.length);
    });
}
// END: Xoá author vào danh sách author =))


document.addEventListener("DOMContentLoaded", initSlider);

