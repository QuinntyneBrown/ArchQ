import { test, expect } from '@playwright/test';

test.describe('Registration Response Time', () => {

  test('should complete new user registration within 30 seconds', async ({ request }) => {
    const ts = Date.now();
    const email = `timeout-test-${ts}@example.com`;

    const start = Date.now();
    const response = await request.post('http://localhost:5299/api/auth/register', {
      data: {
        fullName: 'Timeout Test',
        email,
        password: 'S3cur3P@ss!',
        organizationName: `TimeoutCorp${ts}`,
      },
      timeout: 30000,
    });
    const elapsed = Date.now() - start;

    // Registration must return a response (not hang)
    expect(response.status()).toBe(201);

    // Should complete within 30 seconds
    expect(elapsed).toBeLessThan(30000);
  });
});
