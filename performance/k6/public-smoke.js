import http from 'k6/http';
import { check, sleep } from 'k6';

const baseUrl = __ENV.TEDILE_BASE_URL || 'https://tedile.in';

export const options = {
  vus: Number(__ENV.K6_VUS || 2),
  duration: __ENV.K6_DURATION || '20s',
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<1500'],
  },
};

export default function () {
  const health = http.get(`${baseUrl}/health`);
  check(health, {
    'health is 200': (r) => r.status === 200,
    'health says healthy': (r) => r.json('status') === 'healthy',
  });

  const home = http.get(`${baseUrl}/`);
  check(home, {
    'home is 200': (r) => r.status === 200,
    'home contains Tedile': (r) => r.body.includes('Tedile'),
  });

  sleep(1);
}
