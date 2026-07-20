// ═══════════════════════════════════════════════════════════════
// NOTIFICATION SYSTEM
// ═══════════════════════════════════════════════════════════════
function showNotification(message, type = 'success') {
    document.querySelectorAll('.notification').forEach(n => n.remove());

    const colors = {
        success: { bg: 'var(--ec-success)', icon: '✓' },
        error: { bg: 'var(--ec-danger)', icon: '✗' },
        info: { bg: 'var(--ec-brass-deep)', icon: 'ℹ' },
        warning: { bg: '#f59e0b', icon: '⚠' }
    };
    const { bg, icon } = colors[type] || colors.success;

    const el = document.createElement('div');
    el.className = 'notification';
    el.innerHTML = `<span class="notification-icon">${icon}</span><span>${message}</span>`;
    el.style.cssText = `
        position:fixed;top:1.25rem;left:50%;
        transform:translateX(-50%) translateY(-120%);
        background:${bg};color:white;padding:0.875rem 1.5rem;
        border-radius:0.75rem;box-shadow:0 10px 25px rgba(0,0,0,0.2);
        display:flex;align-items:center;gap:0.6rem;
        font-size:0.95rem;font-weight:600;z-index:9999;
        transition:transform 0.35s cubic-bezier(0.34,1.56,0.64,1);
        white-space:normal;max-width:min(90vw, 26rem);width:max-content;`;
    el.querySelector('.notification-icon').style.cssText = `
        width:1.4rem;height:1.4rem;background:rgba(255,255,255,0.25);
        border-radius:50%;display:flex;align-items:center;
        justify-content:center;font-size:0.8rem;flex-shrink:0;`;
    document.body.appendChild(el);
    requestAnimationFrame(() => { el.style.transform = 'translateX(-50%) translateY(0)'; });
    setTimeout(() => {
        el.style.transform = 'translateX(-50%) translateY(-120%)';
        setTimeout(() => el.remove(), 400);
    }, 3000);
}

// ═══════════════════════════════════════════════════════════════
// CONFIRM DIALOG
// ═══════════════════════════════════════════════════════════════
function showConfirm(message, onConfirm) {
    document.querySelectorAll('.confirm-modal').forEach(n => n.remove());
    const el = document.createElement('div');
    el.className = 'confirm-modal';
    el.style.cssText = `
        position:fixed;inset:0;z-index:9999;
        display:flex;align-items:center;justify-content:center;
        background:rgba(0,0,0,0.4);backdrop-filter:blur(4px);`;
    el.innerHTML = `
        <div style="background:var(--ec-surface);border-radius:var(--ec-radius-lg);padding:2rem;max-width:360px;
                    width:90%;box-shadow:var(--ec-shadow-lg);text-align:center;">
            <div style="width:3rem;height:3rem;background:var(--ec-brass-light);border-radius:50%;
                        display:flex;align-items:center;justify-content:center;
                        margin:0 auto 1rem;font-size:1.25rem;">👤</div>
            <p style="color:var(--ec-text);font-weight:600;font-size:1rem;margin-bottom:0.5rem;">${message}</p>
            <p style="color:var(--ec-text-soft);font-size:0.875rem;margin-bottom:1.5rem;">This will clear your session.</p>
            <div style="display:flex;gap:0.75rem;">
                <button id="confirmCancel" style="flex:1;padding:0.75rem;border:1.5px solid var(--ec-border);
                        border-radius:var(--ec-radius-sm);background:none;font-size:0.95rem;font-weight:600;cursor:pointer;">
                    Cancel
                </button>
                <button id="confirmOk" style="flex:1;padding:0.75rem;border:none;border-radius:var(--ec-radius-sm);
                        background:var(--ec-danger);color:white;font-size:0.95rem;font-weight:600;cursor:pointer;">
                    Logout
                </button>
            </div>
        </div>`;
    document.body.appendChild(el);
    el.querySelector('#confirmOk').addEventListener('click', () => { el.remove(); onConfirm(); });
    el.querySelector('#confirmCancel').addEventListener('click', () => el.remove());
    el.addEventListener('click', e => { if (e.target === el) el.remove(); });
}

// ═══════════════════════════════════════════════════════════════
// SHARED STATE
// ═══════════════════════════════════════════════════════════════
let cartItems = [];
let favoriteItems = [];
let selectedProduct = null;
let isSignUpMode = false;

// Populated from _Layout server-render; JS reads it to avoid an extra AJAX call
const userBtn = document.getElementById('userBtn');
const isAuthenticated = userBtn?.classList.contains('is-authenticated') ?? false;
const serverDisplayName = userBtn?.dataset.displayName ?? null;

// ═══════════════════════════════════════════════════════════════
// MODAL HELPERS
// ═══════════════════════════════════════════════════════════════
function openModal(id) { document.getElementById(id).classList.remove('hidden'); document.body.style.overflow = 'hidden'; }
function closeModal(id) { document.getElementById(id).classList.add('hidden'); document.body.style.overflow = ''; }

function toggleAuthMode() {
    isSignUpMode = !isSignUpMode;
    const modal = document.getElementById('authModal');
    const panel = document.getElementById('authPanel');
    const title = document.getElementById('panelTitle');
    const text = document.getElementById('panelText');
    const btn = document.getElementById('toggleAuthBtn');
    const mText = document.getElementById('mobileToggleText');
    const mBtn = document.getElementById('mobileToggleBtn');
    if (isSignUpMode) {
        panel.classList.add('sign-up-mode');
        modal?.classList.add('sign-up-mode'); // drives the mobile single-form flip (CSS)
        title.textContent = 'Welcome Back!';
        text.textContent = 'To keep connected with us please login with your personal info';
        btn.textContent = 'Sign In';
        mText.textContent = 'Already have an account?';
        mBtn.textContent = 'Sign In';
    } else {
        panel.classList.remove('sign-up-mode');
        modal?.classList.remove('sign-up-mode');
        title.textContent = 'Hello, Friend!';
        text.textContent = 'Enter your personal details and start your journey with us';
        btn.textContent = 'Sign Up';
        mText.textContent = "Don't have an account?";
        mBtn.textContent = 'Sign Up';
    }
}

// ═══════════════════════════════════════════════════════════════
// ANTI-FORGERY HELPER
// ═══════════════════════════════════════════════════════════════
function csrfHeaders() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    return token ? { 'RequestVerificationToken': token } : {};
}

async function apiFetch(url, options = {}) {
    const res = await fetch(url, {
        ...options,
        headers: { 'Content-Type': 'application/json', ...csrfHeaders(), ...(options.headers ?? {}) }
    });
    if (res.status === 204) return null;
    const data = await res.json().catch(() => null);
    if (!res.ok) throw new Error(data?.message ?? res.statusText);
    return data;
}

// ═══════════════════════════════════════════════════════════════
// USER BUTTON STATE  (driven by server-rendered class/data attr)
// ═══════════════════════════════════════════════════════════════
function initUserButton() {
    if (!userBtn) return;
    if (isAuthenticated) {
        userBtn.style.color = 'var(--ec-brass)';
        userBtn.title = serverDisplayName ?? 'Account';
    }
    userBtn.addEventListener('click', () => {
        if (isAuthenticated) {
            showConfirm(`Logged in as ${serverDisplayName}. Do you want to logout?`, doLogout);
        } else {
            openModal('signUpModal');
        }
    });
}

async function doLogout() {
    await apiFetch('/Account/Logout', { method: 'POST' });
    window.location.reload();
}

// ═══════════════════════════════════════════════════════════════
// CART API CALLS
// ═══════════════════════════════════════════════════════════════
async function fetchCart() {
    if (!isAuthenticated) { cartItems = []; updateBadges(); return; }
    try {
        const cart = await apiFetch('/Cart');
        cartItems = cart?.items ?? [];
        updateBadges();
    } catch { cartItems = []; }
}

async function apiAddToCart(productId, quantity = 1) {
    if (!isAuthenticated) { openModal('signUpModal'); return false; }
    const cart = await apiFetch('/Cart/AddItem', {
        method: 'POST',
        body: JSON.stringify({ productId, quantity })
    });
    cartItems = cart?.items ?? [];
    updateBadges();
    return true;
}

async function apiUpdateCartItem(cartItemId, quantity) {
    const cart = await apiFetch('/Cart/UpdateItem', {
        method: 'POST',
        body: JSON.stringify({ cartItemId, quantity })
    });
    cartItems = cart?.items ?? [];
    updateBadges();
}

async function apiRemoveCartItem(cartItemId) {
    const cart = await apiFetch('/Cart/RemoveItem', {
        method: 'POST',
        body: JSON.stringify({ cartItemId })
    });
    cartItems = cart?.items ?? [];
    updateBadges();
}

async function apiCreateOrder() {
    if (!isAuthenticated) { openModal('signUpModal'); return; }
    try {
        await apiFetch('/Orders/CreateFromCart', { method: 'POST' });
        cartItems = [];
        updateBadges();
        renderCart();
        closeModal('cartModal');
        showNotification('Order placed successfully! 🛍️');
    } catch (err) {
        showNotification(err.message, 'error');
    }
}

// ═══════════════════════════════════════════════════════════════
// BADGE COUNTER
// ═══════════════════════════════════════════════════════════════
function updateBadges() {
    const cartCount = cartItems.reduce((s, i) => s + i.quantity, 0);
    const favCount = favoriteItems.length;

    const cartBadge = document.getElementById('cartCount');
    const favBadge = document.getElementById('favoritesCount');
    if (cartBadge) { cartBadge.textContent = cartCount; cartCount > 0 ? cartBadge.classList.remove('hidden') : cartBadge.classList.add('hidden'); }
    if (favBadge) { favBadge.textContent = favCount; favCount > 0 ? favBadge.classList.remove('hidden') : favBadge.classList.add('hidden'); }
}

// ═══════════════════════════════════════════════════════════════
// ICON / STAR HELPERS
// ═══════════════════════════════════════════════════════════════
const SVG_ICONS = {
    frame: '<rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="9" y1="3" x2="9" y2="21"></line><line x1="15" y1="3" x2="15" y2="21"></line><line x1="3" y1="9" x2="21" y2="9"></line><line x1="3" y1="15" x2="21" y2="15"></line>',
    baby: '<path d="M9 12h.01"></path><path d="M15 12h.01"></path><path d="M10 16c.5.3 1.2.5 2 .5s1.5-.2 2-.5"></path><path d="M19 6.3a9 9 0 0 1 1.8 3.9 2 2 0 0 1 0 3.6 9 9 0 0 1-17.6 0 2 2 0 0 1 0-3.6A9 9 0 0 1 12 3c2 0 3.5 1.1 3.5 2.5s-.9 2.5-2 2.5c-.8 0-1.5-.4-1.5-1"></path>',
    dumbbell: '<path d="m6.5 6.5 11 11"></path><path d="m21 21-1-1"></path><path d="m3 3 1 1"></path><path d="m18 22 4-4"></path><path d="m2 6 4-4"></path><path d="m3 10 7-7"></path><path d="m14 21 7-7"></path>',
    zap: '<polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon>',
    mountain: '<path d="m8 3 4 8 5-5 5 15H2L8 3z"></path>'
};
function getIcon(name) { return SVG_ICONS[name] ?? SVG_ICONS.frame; }

function createStarRating(rating) {
    return Array.from({ length: 5 }, (_, i) =>
        `<svg class="star-icon ${i < Math.floor(rating) ? 'filled' : ''}" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
        </svg>`
    ).join('');
}

// ═══════════════════════════════════════════════════════════════
// CART RENDER
// ═══════════════════════════════════════════════════════════════
function renderCart() {
    const content = document.getElementById('cartContent');
    const footer = document.getElementById('cartFooter');
    if (!content) return;

    if (!isAuthenticated) {
        content.innerHTML = `
            <div class="empty-state">
                <p>Please sign in to view your cart</p>
                <button class="btn-primary" onclick="closeModal('cartModal');openModal('signUpModal')">Sign In</button>
            </div>`;
        footer?.classList.add('hidden');
        return;
    }

    if (!cartItems.length) {
        content.innerHTML = `
            <div class="empty-state">
                <p>Your cart is empty</p>
                <p>Add some products to get started</p>
            </div>`;
        footer?.classList.add('hidden');
        return;
    }

    content.innerHTML = `
        <div class="cart-items">
            ${cartItems.map(item => `
                <div class="cart-item">
                    <div class="cart-item-image">
                        <img src="${item.productImage || 'https://images.unsplash.com/photo-1465161191540-aac346fcbaff?w=400'}" alt="${item.productName}">
                    </div>
                    <div class="cart-item-details">
                        <h3 class="cart-item-name">${item.productName}</h3>
                        <p class="cart-item-price">$${item.unitPrice.toFixed(2)}</p>
                        <div class="cart-item-actions">
                            <button class="quantity-btn" onclick="updateCartQty('${item.id}',${Math.max(1, item.quantity - 1)})">
                                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                            </button>
                            <span class="quantity-display">${item.quantity}</span>
                            <button class="quantity-btn" onclick="updateCartQty('${item.id}',${item.quantity + 1})">
                                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                    <line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line>
                                </svg>
                            </button>
                            <button class="remove-btn" onclick="removeFromCart('${item.id}')">
                                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                    <polyline points="3 6 5 6 21 6"></polyline>
                                    <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                                </svg>
                            </button>
                        </div>
                    </div>
                </div>`).join('')}
        </div>`;

    const sub = cartItems.reduce((s, i) => s + i.unitPrice * i.quantity, 0);
    const tax = sub * 0.1;
    const total = sub + tax;
    document.getElementById('cartSubtotal').textContent = `$${sub.toFixed(2)}`;
    document.getElementById('cartTax').textContent = `$${tax.toFixed(2)}`;
    document.getElementById('cartTotal').textContent = `$${total.toFixed(2)}`;
    footer?.classList.remove('hidden');
}

// ═══════════════════════════════════════════════════════════════
// FAVORITES RENDER
// ═══════════════════════════════════════════════════════════════
function renderFavorites() {
    const el = document.getElementById('favoritesContent');
    if (!el) return;

    if (!favoriteItems.length) {
        el.innerHTML = `
            <div class="empty-state">
                <p>No favorites yet</p>
                <p>Add products you love to see them here</p>
            </div>`;
        return;
    }

    el.innerHTML = `
        <div class="favorites-items">
            ${favoriteItems.map(item => `
                <div class="favorite-item">
                    <div class="favorite-item-header">
                        <div class="favorite-item-image">
                            <img src="${item.imageUrl || 'https://images.unsplash.com/photo-1465161191540-aac346fcbaff?w=400'}" alt="${item.name}">
                        </div>
                        <div class="favorite-item-details">
                            <h3 class="favorite-item-name">${item.name}</h3>
                            <p class="favorite-item-price">$${item.price.toFixed(2)}</p>
                            <div class="favorite-item-rating">
                                ${createStarRating(item.rating)}
                                <span>(${item.reviewsCount})</span>
                            </div>
                        </div>
                    </div>
                    <div class="favorite-item-actions">
                        <button class="favorite-add-to-cart-btn" onclick="addToCartFromFavorites('${item.id}')">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
                                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                            </svg>
                            Add to Cart
                        </button>
                        <button class="favorite-remove-btn" onclick="removeFromFavorites('${item.id}')">Remove</button>
                    </div>
                </div>`).join('')}
        </div>`;
}

// ═══════════════════════════════════════════════════════════════
// PRODUCT DETAILS RENDER
// ═══════════════════════════════════════════════════════════════
function renderProductDetails(product) {
    const colors = [
        { name: 'Black', value: '#1a1a1a' },
        { name: 'White', value: '#ffffff' },
        { name: 'Brown', value: '#8B4513' },
        { name: 'Gold', value: '#FFD700' }
    ];
    const sizes = ['XS', 'S', 'M', 'L', 'XL'];
    const isFav = favoriteItems.some(f => f.id === product.id);
    const content = document.getElementById('productDetailsContent');
    if (!content) return;

    content.innerHTML = `
        <div class="details-layout">
            <div class="details-image-section">
                <img src="${product.imageUrl || 'https://images.unsplash.com/photo-1465161191540-aac346fcbaff?w=400'}"
                     alt="${product.name}" class="details-image">
                <button class="details-favorite-btn ${isFav ? 'active' : ''}"
                        onclick="toggleFavoriteFromDetails('${product.id}')">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                        <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                    </svg>
                </button>
            </div>
            <div class="details-info-section">
                <h2 class="details-title">${product.name}</h2>
                <div class="details-rating">
                    <div class="details-stars">${createStarRating(product.rating)}</div>
                    <span class="details-rating-text">${product.rating} (${product.reviewsCount} reviews)</span>
                </div>
                <p class="details-price">$${product.price.toFixed(2)}</p>
                <div class="details-section">
                    <h3>Product Description</h3>
                    <p class="details-description">${product.description || 'Premium quality product designed with attention to detail.'}</p>
                </div>
                <div class="details-section">
                    <h3>Color</h3>
                    <div class="color-options">
                        ${colors.map((c, i) => `
                            <button class="color-option ${i === 0 ? 'active' : ''}" onclick="selectColor(this)" title="${c.name}">
                                <div class="color-option-inner ${c.name === 'White' ? 'white' : ''}" style="background-color:${c.value}"></div>
                            </button>`).join('')}
                    </div>
                </div>
                <div class="details-section">
                    <h3>Size</h3>
                    <div class="size-options">
                        ${sizes.map(s => `<button class="size-option ${s === 'M' ? 'active' : ''}" onclick="selectSize(this)">${s}</button>`).join('')}
                    </div>
                </div>
                <div class="details-section">
                    <h3>Quantity</h3>
                    <div class="quantity-selector">
                        <button onclick="updateDetailsQty(-1)">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                        </button>
                        <span id="detailsQuantity">1</span>
                        <button onclick="updateDetailsQty(1)">
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line>
                            </svg>
                        </button>
                    </div>
                </div>
                <button class="details-add-to-cart-btn" onclick="addToCartFromDetails('${product.id}')">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
                        <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
                        <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                    </svg>
                    <span id="detailsCartText">Add to Cart - $${product.price.toFixed(2)}</span>
                </button>
                <div class="details-additional-info">
                    <div class="info-item">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><polyline points="20 6 9 17 4 12"></polyline></svg>
                        <span>Free shipping on orders over $50</span>
                    </div>
                    <div class="info-item">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><polyline points="20 6 9 17 4 12"></polyline></svg>
                        <span>30-day return policy</span>
                    </div>
                    <div class="info-item">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><polyline points="20 6 9 17 4 12"></polyline></svg>
                        <span>Secure checkout</span>
                    </div>
                </div>
            </div>
        </div>`;
}

// ═══════════════════════════════════════════════════════════════
// SHARED ACTION FUNCTIONS  (called from inline onclick and home.js)
// ═══════════════════════════════════════════════════════════════
async function addToCart(productId, quantity = 1) {
    const ok = await apiAddToCart(productId, quantity);
    if (ok) { renderCart(); showNotification('Added to cart! 🛒'); }
}

async function updateCartQty(cartItemId, quantity) {
    await apiUpdateCartItem(cartItemId, quantity);
    renderCart();
}

async function removeFromCart(cartItemId) {
    await apiRemoveCartItem(cartItemId);
    renderCart();
    showNotification('Item removed from cart', 'info');
}

function removeFromFavorites(productId) {
    favoriteItems = favoriteItems.filter(f => f.id !== productId);
    updateBadges();
    renderFavorites();
    showNotification('Removed from favorites', 'info');
}

function addToCartFromFavorites(productId) {
    addToCart(productId);
    closeModal('favoritesModal');
}

function addToCartFromDetails(productId) {
    const qty = parseInt(document.getElementById('detailsQuantity').textContent);
    addToCart(productId, qty);
    closeModal('productDetailsModal');
}

function updateDetailsQty(delta) {
    const el = document.getElementById('detailsQuantity');
    const qty = Math.max(1, parseInt(el.textContent) + delta);
    el.textContent = qty;
    if (selectedProduct) {
        document.getElementById('detailsCartText').textContent =
            `Add to Cart - $${(selectedProduct.price * qty).toFixed(2)}`;
    }
}

function selectColor(btn) {
    btn.parentElement.querySelectorAll('.color-option').forEach(o => o.classList.remove('active'));
    btn.classList.add('active');
}
function selectSize(btn) {
    btn.parentElement.querySelectorAll('.size-option').forEach(o => o.classList.remove('active'));
    btn.classList.add('active');
}

// ═══════════════════════════════════════════════════════════════
// DOM READY — wire up all shared event listeners
// ═══════════════════════════════════════════════════════════════
document.addEventListener('DOMContentLoaded', async () => {
    initUserButton();

    if (isAuthenticated) await fetchCart();
    updateBadges();
    renderCart();

    // Header buttons
    document.getElementById('favoritesBtn')?.addEventListener('click', () => {
        renderFavorites(); openModal('favoritesModal');
    });
    document.getElementById('cartBtn')?.addEventListener('click', () => {
        renderCart(); openModal('cartModal');
    });

    // Modal close buttons
    document.getElementById('closeSignUpModal')?.addEventListener('click', () => closeModal('signUpModal'));
    document.getElementById('closeCartModal')?.addEventListener('click', () => closeModal('cartModal'));
    document.getElementById('closeFavoritesModal')?.addEventListener('click', () => closeModal('favoritesModal'));
    document.getElementById('closeProductDetailsModal')?.addEventListener('click', () => closeModal('productDetailsModal'));

    // Cart checkout
    document.getElementById('checkoutBtn')?.addEventListener('click', apiCreateOrder);

    // Auth panel toggle
    document.getElementById('toggleAuthBtn')?.addEventListener('click', toggleAuthMode);
    document.getElementById('mobileToggleBtn')?.addEventListener('click', toggleAuthMode);

    // Sign In form
    document.getElementById('signInForm')?.addEventListener('submit', async e => {
        e.preventDefault();
        try {
            await apiFetch('/Account/Login', {
                method: 'POST',
                body: JSON.stringify({
                    email: document.getElementById('loginEmail').value,
                    password: document.getElementById('loginPassword').value
                })
            });
            closeModal('signUpModal');
            window.location.reload(); // refresh so _Layout re-renders server-side auth state
        } catch (err) {
            showNotification(err.message, 'error');
        }
    });

    // Sign Up form
    document.getElementById('signUpForm')?.addEventListener('submit', async e => {
        e.preventDefault();
        const password = document.getElementById('registerPassword').value;
        const confirmPassword = document.getElementById('registerConfirmPassword').value;
        if (password !== confirmPassword) { showNotification('Passwords do not match', 'error'); return; }
        try {
            await apiFetch('/Account/Register', {
                method: 'POST',
                body: JSON.stringify({
                    displayName: document.getElementById('registerName').value,
                    email: document.getElementById('registerEmail').value,
                    password,
                    confirmPassword
                })
            });
            closeModal('signUpModal');
            window.location.reload();
        } catch (err) {
            showNotification(err.message, 'error');
        }
    });

    // Backdrop click to close modals
    document.querySelectorAll('.modal-backdrop').forEach(bd => {
        bd.addEventListener('click', e => {
            const modal = e.target.closest('.modal');
            if (modal) closeModal(modal.id);
        });
    });
});