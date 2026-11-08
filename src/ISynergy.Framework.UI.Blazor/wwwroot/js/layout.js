export function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

export function isDarkMode() {
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
}