export const TEST_USERS = {
  admin: {
    email: 'admin@acme-corp.test',
    password: 'Test@12345678',
    name: 'Admin User',
    role: 'Admin',
  },
  author: {
    email: 'author@acme-corp.test',
    password: 'Test@12345678',
    name: 'Alice Chen',
    role: 'Author',
  },
  reviewer: {
    email: 'reviewer@acme-corp.test',
    password: 'Test@12345678',
    name: 'Bob Kim',
    role: 'Reviewer',
  },
  viewer: {
    email: 'viewer@acme-corp.test',
    password: 'Test@12345678',
    name: 'Carol Mata',
    role: 'Viewer',
  },
} as const;

export const TEST_TENANTS = {
  primary: {
    name: 'Acme Corp',
    slug: 'acme-corp',
  },
  secondary: {
    name: 'Beta Inc',
    slug: 'beta-inc',
  },
} as const;

export const TEST_ADRS = {
  draft: {
    title: 'Use Event-Driven Architecture for Order Processing',
    content: '## Context\n\nOur order processing pipeline...\n\n## Decision\n\nWe will adopt an event-driven architecture...\n\n## Consequences\n\nPositive: Decoupled services...',
  },
  inReview: {
    title: 'Adopt PostgreSQL as Primary Database',
    content: '## Context\n\nOur application currently uses...\n\n## Decision\n\nWe will adopt PostgreSQL...\n\n## Consequences\n\nPositive: Unified database...',
  },
} as const;
