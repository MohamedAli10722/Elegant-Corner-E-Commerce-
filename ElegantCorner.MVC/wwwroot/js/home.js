// ═══════════════════════════════════════════════════════════════
// HOME PAGE — reads data injected by the Razor view
// ═══════════════════════════════════════════════════════════════
let categories       = window.__CATEGORIES__ ?? [];
let products         = window.__PRODUCTS__   ?? [];
let selectedCategory = null;

// ═══════════════════════════════════════════════════════════════
// RENDER CATEGORIES
// ═══════════════════════════════════════════════════════════════
function renderCategories() {
    const mobileEl   = document.getElementById('categoriesMobile');
    const desktopEl  = document.getElementById('categoriesDesktop');
    const viewAllBtn = document.getElementById('viewAllBtn');
    if (!mobileEl || !desktopEl) return;

    mobileEl.innerHTML = categories.map(cat => `
        <button class="category-btn-mobile ${selectedCategory === cat.id ? 'active' : ''}"
                onclick="selectCategory('${cat.id}')">
            <svg class="category-icon-mobile" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                ${getIcon(cat.icon)}
            </svg>
            <span class="category-name-mobile">${cat.name}</span>
        </button>`).join('');

    desktopEl.innerHTML = categories.map(cat => `
        <button class="category-card ${selectedCategory === cat.id ? 'active' : ''}"
                onclick="selectCategory('${cat.id}')">
            <div class="category-image-wrapper">
                <img src="${cat.imageUrl || 'https://images.unsplash.com/photo-1465161191540-aac346fcbaff?w=400'}"
                     alt="${cat.name}" class="category-image">
                <div class="category-overlay"></div>
                <div class="category-content">
                    <svg class="category-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                        ${getIcon(cat.icon)}
                    </svg>
                    <h3 class="category-name">${cat.name}</h3>
                </div>
            </div>
        </button>`).join('');

    selectedCategory ? viewAllBtn?.classList.remove('hidden') : viewAllBtn?.classList.add('hidden');
}

// ═══════════════════════════════════════════════════════════════
// RENDER PRODUCTS GRID  (replaces server-rendered HTML after filter)
// ═══════════════════════════════════════════════════════════════
function renderProducts() {
    const grid     = document.getElementById('productsGrid');
    const titleEl  = document.getElementById('productsTitle');
    const countEl  = document.getElementById('productsCount');
    if (!grid) return;

    if (selectedCategory) {
        const cat = categories.find(c => c.id === selectedCategory);
        titleEl.textContent = `${cat?.name ?? ''} Products`;
    } else {
        titleEl.textContent = 'All Products';
    }
    countEl.textContent = `${products.length} products available`;

    grid.innerHTML = products.map(p => {
        const isFav = favoriteItems.some(f => f.id === p.id);
        return `
            <div class="product-card">
                <div class="product-image-wrapper" onclick="showProductDetails('${p.id}')">
                    <img src="${p.imageUrl || 'https://images.unsplash.com/photo-1465161191540-aac346fcbaff?w=400'}"
                         alt="${p.name}" class="product-image">
                    <button class="product-favorite-btn ${isFav ? 'active' : ''}"
                            onclick="event.stopPropagation(); homeToggleFavorite('${p.id}')">
                        <svg class="product-favorite-icon ${isFav ? 'active' : ''}" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                        </svg>
                    </button>
                </div>
                <div class="product-info">
                    <h3 class="product-name" onclick="showProductDetails('${p.id}')">${p.name}</h3>
                    <div class="product-rating">
                        ${createStarRating(p.rating)}
                        <span class="product-reviews">(${p.reviewsCount})</span>
                    </div>
                    <div class="product-footer">
                        <span class="product-price">$${p.price.toFixed(2)}</span>
                        <button class="product-cart-btn" onclick="addToCart('${p.id}')">
                            <svg class="product-cart-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <circle cx="9" cy="21" r="1"></circle>
                                <circle cx="20" cy="21" r="1"></circle>
                                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>`;
    }).join('');
}

// ═══════════════════════════════════════════════════════════════
// HOME-SPECIFIC ACTIONS
// ═══════════════════════════════════════════════════════════════

// Thin wrapper: toggles favorite then re-renders products to update heart icon state
function homeToggleFavorite(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;
    const idx = favoriteItems.findIndex(f => f.id === productId);
    if (idx > -1) {
        favoriteItems.splice(idx, 1);
        showNotification('Removed from favorites', 'info');
    } else {
        favoriteItems.push(product);
        showNotification('Added to favorites! ❤️');
    }
    updateBadges();
    renderProducts(); // re-render to flip heart icon state
}

function toggleFavoriteFromDetails(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;
    const idx = favoriteItems.findIndex(f => f.id === productId);
    idx > -1 ? favoriteItems.splice(idx, 1) : favoriteItems.push(product);
    updateBadges();
    renderProductDetails(product); // re-render modal with updated heart state
}

function showProductDetails(productId) {
    selectedProduct = products.find(p => p.id === productId);
    if (!selectedProduct) return;
    renderProductDetails(selectedProduct);
    openModal('productDetailsModal');
}

// Category click: toggle selection, AJAX-fetch filtered products
async function selectCategory(categoryId) {
    selectedCategory = (selectedCategory === categoryId) ? null : categoryId;
    renderCategories();

    const url      = selectedCategory ? `/Home/Products?categoryId=${selectedCategory}` : '/Home/Products';
    const response = await fetch(url);
    products       = await response.json();
    renderProducts();
}

// ═══════════════════════════════════════════════════════════════
// INIT
// ═══════════════════════════════════════════════════════════════
document.addEventListener('DOMContentLoaded', () => {
    renderCategories();

    // Update count text to match server-rendered product count
    const countEl = document.getElementById('productsCount');
    if (countEl) countEl.textContent = `${products.length} products available`;

    // View All button resets category filter
    document.getElementById('viewAllBtn')?.addEventListener('click', async () => {
        selectedCategory = null;
        renderCategories();
        const res = await fetch('/Home/Products');
        products   = await res.json();
        renderProducts();
    });
});
