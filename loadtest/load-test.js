import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 25 }, // Ramp-up to 25 users
        { duration: '1m', target: 25 },  // Stay at 25 users for 1 minute
        { duration: '30s', target: 0 }, // Ramp-down to 0
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
    },
};

// const BASE_URL = 'http://localhost:5000';
const BASE_URL = 'http://host.docker.internal';


export default function () {
    // 1. Visit Homepage
    let res = http.get(`${BASE_URL}/`);
    check(res, { 'is status 200': (r) => r.status === 200 });
    sleep(1);

    // 2. Search for products (triggers Search instrumentation)
    const keywords = ['computer', 'notebook', 'phone', 'shirt', 'shoes'];
    const randomKeyword = keywords[Math.floor(Math.random() * keywords.length)];

    res = http.get(`${BASE_URL}/search?q=${randomKeyword}`);
    check(res, { 'search title contains keyword': (r) => r.status === 200 });
    sleep(2);

    // 3. View a product (triggers ProductDetails instrumentation)
    const productSlugs = [
        'adidas-consortium-campus-80s-running-shoes',
        'ray-ban-aviator-sunglasses',
        'first-prize-pies',
        'flower-girl-bracelet',
        'night-visions'
    ];
    const randomSlug = productSlugs[Math.floor(Math.random() * productSlugs.length)];

    res = http.get(`${BASE_URL}/${randomSlug}`);
    check(res, { 'product page loaded': (r) => r.status === 200 });

    // 4. Go to non existent product
    // if (Math.random() > 0.8) {
    //     res = http.get(`${BASE_URL}/non-existent-999`);
    //     check(res, { 'is status 404': (r) => r.status === 404 });
    // }

    sleep(1);
}
