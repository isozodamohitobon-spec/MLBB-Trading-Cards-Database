
let currentCardCode = "";

function openModal(cardCode) {
    currentCardCode = cardCode;

    fetch('/api/card/' + encodeURIComponent(cardCode))
        .then(response => {
            if (!response.ok) {
                throw new Error('Карточка не найдена');
            }

            return response.json();
        })
        .then(data => {
            document.getElementById('modalImage').src =
                data.imageUrl || 'https://via.placeholder.com/500x300?text=No+Image';

            document.getElementById('modalTitle').innerText = data.heroName;
            document.getElementById('modalSkin').innerText = data.cardName;
            document.getElementById('modalId').innerText = data.cardCode;
            document.getElementById('modalRarity').innerText = data.rarityName;

            document.getElementById('modalRarityBadge').innerText =
                data.rarityName + ' • ' +
                (data.rarityName === 'UR' ? 'Ultra Rare' :
                data.rarityName === 'RS' ? 'Rare Shiny' :
                data.rarityName === 'QR' ? 'Special Card' : 'Rare');

            document.getElementById('modalPrice').innerText = '$' + data.price;
            document.getElementById('modalViews').innerText = data.timesViewed;

            document.getElementById('cardModal').style.display = 'flex';
        })
        .catch(error => {
            alert('Ошибка загрузки данных');
            console.error(error);
        });
}

function closeModal() {
    document.getElementById('cardModal').style.display = 'none';
}

function deleteCard() {
    if (confirm('Удалить ' + currentCardCode + '?')) {
        window.location.href =
            '/api/delete/' + encodeURIComponent(currentCardCode);
    }
}

// Поиск, фильтрация и сортировка
document.addEventListener('DOMContentLoaded', () => {

    const searchInput = document.getElementById('searchInput');
    const rarityFilter = document.getElementById('rarityFilter');
    const cardGrid = document.getElementById('cardGrid');

    const sortAll = document.getElementById('sortAll');
    const sortPrice = document.getElementById('sortPrice');
    const sortViews = document.getElementById('sortViews');

    let currentSort = 'all';

    // Получаем карточки
    function getCards() {
        return Array.from(cardGrid.querySelectorAll('.card'));
    }

    // Сортировка карточек
    function sortCards() {
        const cards = getCards();

        if (currentSort === 'price') {
            cards.sort((a, b) => {
                const priceA = parseFloat(
                    a.querySelector('.card-price')?.innerText.replace(/[^0-9.]/g, '') || 0
                );

                const priceB = parseFloat(
                    b.querySelector('.card-price')?.innerText.replace(/[^0-9.]/g, '') || 0
                );

                return priceB - priceA;
            });
        }

        else if (currentSort === 'views') {
            cards.sort((a, b) => {
                const viewsA = Number(a.dataset.views || 0);
                const viewsB = Number(b.dataset.views || 0);

                return viewsB - viewsA;
            });
        }

        cards.forEach(card => cardGrid.appendChild(card));

        filterCards();
    }

    // Поиск и фильтрация
    function filterCards() {
        const searchText = searchInput.value.toLowerCase();
        const rarityText = rarityFilter.value;

        const cards = getCards();

        cards.forEach(card => {
            const cardText = card.innerText.toLowerCase();

            const badge = card.querySelector('.card-top-badge');
            const cardBadge = badge ? badge.innerText.trim() : '';

            const matchesSearch = cardText.includes(searchText);
            const matchesRarity =
                rarityText === "" || cardBadge === rarityText;

            card.style.display =
                matchesSearch && matchesRarity ? 'block' : 'none';
        });
    }

    // Активная кнопка сортировки
    function setActiveButton(button) {
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.classList.remove('active');
        });

        button.classList.add('active');
    }

    // Все карточки
    sortAll.addEventListener('click', () => {
        currentSort = 'all';
        setActiveButton(sortAll);
        sortCards();
    });

    // Самые дорогие
    sortPrice.addEventListener('click', () => {
        currentSort = 'price';
        setActiveButton(sortPrice);
        sortCards();
    });

    // Самые просматриваемые
    sortViews.addEventListener('click', () => {
        currentSort = 'views';
        setActiveButton(sortViews);
        sortCards();
    });

    searchInput.addEventListener('input', filterCards);
    rarityFilter.addEventListener('change', filterCards);

});
