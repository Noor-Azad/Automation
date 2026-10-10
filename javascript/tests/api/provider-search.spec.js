const { test, expect } = require('@playwright/test');

async function search(request, params) {
  const response = await request.get('/api/search/providers', { params });
  expect(response.status()).toBe(200);
  return response.json();
}

test('provider search paging metadata is consistent', async ({ request }) => {
  const data = await search(request, { limit: 1, offset: 0, sort: 'rating-high' });
  expect(data.status).toBe(true);
  expect(data.limit).toBe(1);
  expect(data.offset).toBe(0);
  expect(Array.isArray(data.data.providers)).toBe(true);
  expect(data.count).toBe(data.data.providers.length);
  expect(data.total).toBeGreaterThanOrEqual(data.data.providers.length);
  expect(data.next_offset).toBe(data.total > 1 ? 1 : null);
});

test('consecutive search pages do not repeat provider IDs', async ({ request }) => {
  const first = await search(request, { limit: 1, offset: 0, sort: 'rating-high' });
  if (first.total < 2 || first.data.providers.length === 0) return;
  const second = await search(request, { limit: 1, offset: 1, sort: 'rating-high' });
  expect(second.data.providers.length).toBeGreaterThan(0);
  expect(second.data.providers[0].id).not.toBe(first.data.providers[0].id);
});

test('verified-only search returns only verified providers', async ({ request }) => {
  const data = await search(request, { verified_only: true, limit: 50, offset: 0 });
  for (const provider of data.data.providers) expect(provider.verified).toBe(true);
});

test('active services return unique slugs', async ({ request }) => {
  const response = await request.get('/api/services');
  expect(response.status()).toBe(200);
  const body = await response.json();
  expect(Array.isArray(body.data)).toBe(true);
  expect(body.data.length).toBeGreaterThan(0);
  const slugs = body.data.map(service => service.slug).filter(Boolean).map(s => s.toLowerCase());
  expect(new Set(slugs).size).toBe(slugs.length);
});
