window.dataLayer = window.dataLayer || [];
function gtag() { dataLayer.push(arguments) };

const injectGAScript = (measurementId) => {
    // Load the Google tag manager script dynamically
    const script = document.createElement('script');
    script.async = true;
    script.src = `https://www.googletagmanager.com/gtag/js?id=${measurementId}`;
    document.head.appendChild(script);

    // Initialize GA4 once the script loads
    script.onload = () => {
        gtag('js', new Date());
        gtag('config', measurementId);
        console.log('Google Analytics 4 initialized successfully');
    };

    script.onerror = () => {
        console.error('Failed to load Google Analytics 4');
    };
};
function injectMCScript(projectId) {
    try {
        (function (c, l, a, r, i) {
            if (l.getElementById("clarity-script")) {
                return;
            }
            c[a] = c[a] ||
                function () {
                    (c[a].q = c[a].q || []).push(arguments);
                };
            const script = l.createElement(r);
            script.async = 1;
            script.src = "https://www.clarity.ms/tag/" + i + "?ref=npm";
            script.id = "clarity-script";
            const firstScript = l.getElementsByTagName(r)[0];
            firstScript.parentNode.insertBefore(script, firstScript);
        })(window, document, "clarity", "script", projectId);
        console.log('Microsoft Clarity initialized successfully');
        return;
    } catch (error) {
        console.error('Failed to load Microsoft Clarity');
        return;
    }
};

const Clarity = {
    init(projectId) {
        injectMCScript(projectId);
    },

    setTag(key, value) {
        window.clarity('set', key, value);
    },

    identify(customerId, customSessionId, customPageId, friendlyName) {
        window.clarity('identify', customerId, customSessionId, customPageId, friendlyName);
    },

    consent(consent = true) {
        window.clarity('consent', consent);
    },

    upgrade(reason) {
        window.clarity('upgrade', reason);
    },

    event(eventName) {
        window.clarity('event', eventName);
    },
};

export async function getCookiePolicy() {
    const cookiePolicy = localStorage.getItem("cookie-policy")

    return JSON.parse(cookiePolicy)
}

export async function setCookiePolicy(state) {
    localStorage.setItem("cookie-policy", JSON.stringify(state))
}

export async function initAnalytics(GAmeasurementId, MCprojectId) {
    // Google Analytics initialization
    injectGAScript(GAmeasurementId);

    // Microsoft Clarity initialization
    Clarity.init(MCprojectId)
    Clarity.consent()

    console.log("Analytics initialized")
}