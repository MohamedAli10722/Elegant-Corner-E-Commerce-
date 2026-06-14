// ============================================================
// CONFIG - MVC Version
// ============================================================
// In MVC, we can call the same API endpoints OR use MVC endpoints
// For cart operations, we'll use the API to be consistent
const API_BASE = 'https://localhost:7264/api';

// ============================================================
// NOTIFICATION SYSTEM
// ============================================================
function showNotification(message, type = 'success') {
    document.querySelectorAll('.notification').forEach(n => n.remove());

    const colors = {
        success: { bg: '#10b981', icon: '✓' },
        error: { bg: '#ef4444', icon: '✗' },
        info: { bg: '#2563eb', icon: 'ℹ' },
        warning: { bg: '#f59e0b', icon: '⚠' }
    };

    const { bg, icon } = colors[type] || colors.success;

    const el = document.createElement('div');
    el.style.cssText = `
        position: fixed;
        top: 1.25rem;
        left: 50%;
        transform: translateX(-50%) translateY(-120%);
        background: ${bg};
        color: white;
        padding: 0.875rem 1.5rem;
        border-radius: 0.75rem;
        box-shadow: 0 10px 25px rgba(0,0,0,0.2);
        display: flex;
        align-items: center;
        gap: 0.6rem;
        font-size: 0.95rem;
        font-weight: 600;
        z-index: 9999;
        transition: transform 0.35s cubic-bezier(0.34,1.56,0.64,1);
    `;
    el.innerHTML = `
        <span style="
            width: 1.4rem; height: 1.4rem;
            background: rgba(255,255,255,0.25);
            border-radius: 50%;
            display: flex;
            align-items: center; justify-content: center;
            font-size: 0.8rem;
        ">${icon}</span>
        <span>${message}</span>
    `;

    document.body.appendChild(el);
    requestAnimationFrame(() => {
        el.style.transform = 'translateX(-50%) translateY(0)';
    });

    setTimeout(() => {
        el.style.transform = 'translateX(-50%) translateY(-120%)';
        setTimeout(() => el.remove(), 400);
    }, 3000);
}

// ============================================================
// STATE (MVC)
// ============================================================
let categories = [];
let products = [];
let favoriteItems = [];
let selectedCategory = null;
let selectedProduct = null;

// ============================================================
// LOAD DATA FROM PAGE (MVC Rendered)
// ============================================================
function initializeFromPage() {
    // In MVC, categories and products are already in the DOM
    // We can extract them or rebuild from the page
    // For now, favorites come from localStorage

    const stored = localStorage.getItem('favorites');
    if (stored) {
        favoriteItems = JSON.parse(stored);
    }
    updateBadges();
}

function saveFavorites() {
    localStorage.setItem('favorites', JSON.stringify(favoriteItems));
    updateBadges();
}

// ============================================================
// BADGE UPDATES
// ============================================================
function updateBadges() {
    const favBadge = document.getElementById('favoritesCount');
    if (favBadge) {
        favBadge.textContent = favoriteItems.length;
        if (favoriteItems.length > 0) {
            favBadge.classList.remove('hidden');
        } else {
            favBadge.classList.add('hidden');
        }
    }
}

// ============================================================
// CART OPERATIONS (Still call API)
// ============================================================
async function addToCart(productId, quantity = 1) {
    try {
        const response = await fetch(`${API_BASE}/cart/items`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify({ productId, quantity })
        });

        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/Account/Login';
                return;
            }
            throw new Error('Failed to add to cart');
        }

        showNotification('Added to cart! 🛒');
    } catch (err) {
        showNotification('Please sign in to add items', 'info');
    }
}

async function removeFromCart(cartItemId) {
    try {
        await fetch(`${API_BASE}/cart/items/${cartItemId}`, {
            method: 'DELETE',
            credentials: 'include'
        });
        showNotification('Item removed', 'info');
    } catch (err) {
        showNotification('Error removing item', 'error');
    }
}

// ============================================================
// FAVORITES (Local Storage)
// ============================================================
function toggleFavorite(productId) {
    const index = favoriteItems.findIndex(id => id === productId);
    if (index > -1) {
        favoriteItems.splice(index, 1);
        showNotification('Removed from favorites', 'info');
    } else {
        favoriteItems.push(productId);
        showNotification('Added to favorites! ❤️');
    }
    saveFavorites();
    updateFavoriteButtons();
}

function updateFavoriteButtons() {
    document.querySelectorAll('.product-favorite-btn').forEach(btn => {
        const card = btn.closest('.product-card');
        const productId = card?.onclick?.toString().match(/'([a-f0-9-]+)'/)?.[1];
        if (productId && favoriteItems.includes(productId)) {
            btn.classList.add('active');
            btn.querySelector('.product-favorite-icon').classList.add('active');
        } else {
            btn.classList.remove('active');
            btn.querySelector('.product-favorite-icon').classList.remove('active');
        }
    });
}

// ============================================================
// PRODUCT DETAILS MODAL
// ============================================================
function showProductDetails(productId) {
    const modalContent = document.getElementById('productDetailsContent');

    // In MVC, get product data from page or fetch from API
    fetch(`${API_BASE}/products/${productId}`)
        .then(r => r.json())
        .then(product => {
            const isFavorite = favoriteItems.includes(productId);

            modalContent.innerHTML = `
                <div class="details-layout">
                    <div class="details-image-section">
                        <img src="${product.imageUrl}" alt="${product.name}" class="details-image">
                        <button class="details-favorite-btn ${isFavorite ? 'active' : ''}" 
                            onclick="toggleFavoriteFromDetails('${product.id}')">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                            </svg>
                        </button>
                    </div>
                    <div class="details-info-section">
                        <h2 class="details-title">${product.name}</h2>
                        <p class="details-price">$${product.price.toFixed(2)}</p>
                        <div class="details-section">
                            <h3>Description</h3>
                            <p class="details-description">${product.description || 'Premium product'}</p>
                        </div>
                        <div class="details-section">
                            <h3>Quantity</h3>
                            <div class="quantity-selector">
                                <button onclick="updateDetailsQuantity(-1)">
                                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                        <line x1="5" y1="12" x2="19" y2="12"></line>
                                    </svg>
                                </button>
                                <span id="detailsQuantity">1</span>
                                <button onclick="updateDetailsQuantity(1)">
                                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                        <line x1="12" y1="5" x2="12" y2="19"></line>
                                        <line x1="5" y1="12" x2="19" y2="12"></line>
                                    </svg>
                                </button>
                            </div>
                        </div>
                        <button class="details-add-to-cart-btn" onclick="addToCartFromDetails('${product.id}')">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <circle cx="9" cy="21" r="1"></circle>
                                <circle cx="20" cy="21" r="1"></circle>
                                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                            </svg>
                            <span>Add to Cart - $${product.price.toFixed(2)}</span>
                        </button>
                    </div>
                </div>
            `;

            openModal('productDetailsModal');
        })
        .catch(err => {
            showNotification('Error loading product', 'error');
        });
}

function addToCartFromDetails(productId) {
    const quantity = parseInt(document.getElementById('detailsQuantity')?.textContent || 1);
    addToCart(productId, quantity);
    closeModal('productDetailsModal');
}

function updateDetailsQuantity(delta) {
    const el = document.getElementById('detailsQuantity');
    if (el) {
        el.textContent = Math.max(1, parseInt(el.textContent) + delta);
    }
}

function toggleFavoriteFromDetails(productId) {
    toggleFavorite(productId);
    updateFavoriteButtons();
}

// ============================================================
// MODAL FUNCTIONS
// ============================================================
function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('hidden');
        document.body.style.overflow = '';
    }
}

// ============================================================
// PAGE INITIALIZATION (MVC)
// ============================================================
document.addEventListener('DOMContentLoaded', () => {
    initializeFromPage();

    // Favorites button
    const favBtn = document.getElementById('favoritesBtn');
    if (favBtn) {
        favBtn.addEventListener('click', () => {
            // Show favorites modal or redirect to favorites page
            showNotification('View your favorites in the heart icon!');
        });
    }

    // Cart button
    const cartBtn = document.getElementById('cartBtn');
    if (cartBtn) {
        cartBtn.addEventListener('click', () => {
            openModal('cartModal');
        });
    }

    // Close modals
    document.getElementById('closeCartModal')?.addEventListener('click', () => closeModal('cartModal'));
    document.getElementById('closeFavoritesModal')?.addEventListener('click', () => closeModal('favoritesModal'));
    document.getElementById('closeProductDetailsModal')?.addEventListener('click', () => closeModal('productDetailsModal'));

    // Close modals on backdrop click
    document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
        backdrop.addEventListener('click', (e) => {
            const modal = e.target.closest('.modal');
            if (modal) closeModal(modal.id);
        });
    });

    // User button
    const userBtn = document.getElementById('userBtn');
    if (userBtn) {
        userBtn.addEventListener('click', () => {
            // Show user menu or logout dialog
            showConfirm('Do you want to logout?', () => {
                window.location.href = '/Account/Logout';
            });
        });
    }

    updateFavoriteButtons();
});

// ============================================================
// CONFIRM DIALOG (MVC)
// ============================================================
function showConfirm(message, onConfirm) {
    const el = document.createElement('div');
    el.style.cssText = `
        position: fixed;
        inset: 0;
        z-index: 9999;
        display: flex;
        align-items: center;
        justify-content: center;
        background: rgba(0,0,0,0.4);
        backdrop-filter: blur(4px);
    `;
    el.innerHTML = `
        <div style="
            background: white;
            border-radius: 1rem;
            padding: 2rem;
            max-width: 360px;
            width: 90%;
            box-shadow: 0 25px 50px rgba(0,0,0,0.2);
            text-align: center;
        ">
            <p style="
                color: #111;
                font-weight: 600;
                font-size: 1rem;
                margin-bottom: 1.5rem;
            ">${message}</p>
            <div style="display: flex; gap: 0.75rem;">
                <button id="cancelBtn" style="
                    flex: 1;
                    padding: 0.75rem;
                    border: 1.5px solid #d1d5db;
                    border-radius: 0.5rem;
                    background: none;
                    font-weight: 600;
                    cursor: pointer;
                ">Cancel</button>
                <button id="okBtn" style="
                    flex: 1;
                    padding: 0.75rem;
                    border: none;
                    border-radius: 0.5rem;
                    background: #ef4444;
                    color: white;
                    font-weight: 600;
                    cursor: pointer;
                ">Confirm</button>
            </div>
        </div>
    `;

    document.body.appendChild(el);

    el.querySelector('#okBtn').addEventListener('click', () => {
        el.remove();
        onConfirm();
    });
    el.querySelector('#cancelBtn').addEventListener('click', () => el.remove());
    el.addEventListener('click', (e) => { if (e.target === el) el.remove(); });
}