import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
    http.get('http://localhost:5037/api/kafka');
    sleep(1);
}
