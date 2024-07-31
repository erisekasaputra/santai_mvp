import http from 'k6/http';
import { check } from 'k6';

export const options = {
    scenarios: {
        constant_request_rate: {
            executor: 'constant-arrival-rate',
            rate: 1000, // Number of requests per time unit
            timeUnit: '30s', // Time unit (1000 requests per second)
            duration: '1m', // Duration of the test
            preAllocatedVUs: 100, // Initial number of virtual users
            maxVUs: 200, // Maximum number of virtual users
        },
    },
};

export default function () {
    // Replace with your actual endpoint and request body
    const url = 'http://localhost:5206/api/v1/catalog/items/stock/deduct';
    const payload = JSON.stringify({
        itemStocks: [
            {
                itemId: '01J3NKF5GKP74FJHNBHHV33E0H',
                quantityDeduct: 1,
            },
        ],
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Accept': '*/*',
        },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'status was 200': (r) => r.status === 200 || r.status === 204,
    });
}
