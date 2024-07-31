import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '1m', target: 1000 }, 
        { duration: '5m', target: 1000 }, 
        { duration: '1m', target: 0 }      
    ],
};

export default function () {
    let url = 'http://localhost:5206/api/v1/catalog/items';
    let payload = JSON.stringify({
        name: "Shampo",
        description: "test",
        price: 10000,
        imageUrl: "https://facebook.com",
        stockQuantity: 1,
        soldQuantity: 1
    });
    let headers = { 'Content-Type': 'application/json' };
    let response = http.post(url, payload, { headers: headers });
    check(response, {
        'status is 201': (r) => r.status === 201,
        'response is JSON': (r) => {
            try {
                let result = JSON.parse(r.body); 
                let strings = JSON.stringify(result)

                if (strings.indexOf('"success":true') == -1)
                {
                    console.log(result)
                    return false;
                } 

                return true;
            } catch (e) {   
                return false;
            }
        }
    });
    sleep(0.01); 
}