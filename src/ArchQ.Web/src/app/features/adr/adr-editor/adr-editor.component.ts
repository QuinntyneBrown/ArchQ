import { Component, signal, computed, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { marked } from 'marked';
import { AdrService } from '../../../core/services/adr.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../shared/components/toast/toast.service';

@Component({
  selector: 'app-adr-editor',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="editor-container">
      <!-- Top Bar -->
      <div class="top-bar">
        <div class="top-bar-left">
          <a routerLink="/adrs" class="back-arrow" data-testid="back-button">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="19" y1="12" x2="5" y2="12"></line>
              <polyline points="12 19 5 12 12 5"></polyline>
            </svg>
          </a>
          <span class="top-bar-title">New ADR</span>
          <span class="status-badge" data-testid="status-badge">Draft</span>
        </div>
        <div class="top-bar-right">
          <button
            class="btn-save"
            data-testid="save-draft-button"
            [disabled]="saving()"
            (click)="saveDraft()"
          >
            {{ saving() ? 'Saving...' : 'Save Draft' }}
          </button>
          <button class="btn-submit" disabled>Submit for Review</button>
          <a routerLink="/adrs" class="cancel-link" data-testid="cancel-button">Cancel</a>
        </div>
      </div>

      <!-- Title Input -->
      <div class="title-section">
        <label for="adr-title" class="field-label">ADR Title</label>
        <input
          id="adr-title"
          type="text"
          [(ngModel)]="title"
          name="title"
          data-testid="adr-title-input"
          placeholder="e.g., Use CQRS Pattern for Command/Query Separation"
          class="title-input"
        />
      </div>

      <!-- Mobile Tabs -->
      <div class="mobile-tabs">
        <button
          class="tab-btn"
          [class.active]="activeTab() === 'edit'"
          data-testid="edit-tab"
          (click)="activeTab.set('edit')"
        >Edit</button>
        <button
          class="tab-btn"
          [class.active]="activeTab() === 'preview'"
          data-testid="preview-tab"
          (click)="activeTab.set('preview')"
        >Preview</button>
      </div>

      <!-- Split Pane -->
      <div class="split-pane">
        <!-- Editor Pane -->
        <div class="editor-pane" [class.hidden-mobile]="activeTab() === 'preview'">
          <!-- Toolbar -->
          <div class="toolbar" data-testid="markdown-toolbar">
            <button class="toolbar-btn" title="Bold" (click)="insertMarkdown('bold')"><strong>B</strong></button>
            <button class="toolbar-btn" title="Italic" (click)="insertMarkdown('italic')"><em>I</em></button>
            <button class="toolbar-btn" title="Code" (click)="insertMarkdown('code')">&lt;&gt;</button>
            <button class="toolbar-btn" title="Link" (click)="insertMarkdown('link')">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"></path>
                <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"></path>
              </svg>
            </button>
            <button class="toolbar-btn" title="List" (click)="insertMarkdown('list')">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <line x1="8" y1="6" x2="21" y2="6"></line>
                <line x1="8" y1="12" x2="21" y2="12"></line>
                <line x1="8" y1="18" x2="21" y2="18"></line>
                <line x1="3" y1="6" x2="3.01" y2="6"></line>
                <line x1="3" y1="12" x2="3.01" y2="12"></line>
                <line x1="3" y1="18" x2="3.01" y2="18"></line>
              </svg>
            </button>
            <button class="toolbar-btn" title="Image" (click)="insertMarkdown('image')">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
                <circle cx="8.5" cy="8.5" r="1.5"></circle>
                <polyline points="21 15 16 10 5 21"></polyline>
              </svg>
            </button>
          </div>
          <textarea
            #editorTextarea
            class="markdown-editor"
            [(ngModel)]="content"
            name="content"
            data-testid="markdown-edit-area"
            placeholder="Write your ADR content in Markdown..."
          ></textarea>
        </div>

        <!-- Preview Pane -->
        <div class="preview-pane" [class.hidden-mobile]="activeTab() === 'edit'" data-testid="preview-pane">
          <div class="preview-header">Preview</div>
          <div class="preview-content markdown-body" [innerHTML]="renderedHtml()"></div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .editor-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    /* Top Bar */
    .top-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem 1.5rem;
      background-color: #1a1d2e;
      border-bottom: 1px solid #2a2d3e;
    }
    .top-bar-left {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }
    .back-arrow {
      color: #9ca3af;
      display: flex;
      align-items: center;
      text-decoration: none;
      transition: color 0.15s ease;
    }
    .back-arrow:hover { color: #ffffff; }
    .top-bar-title {
      font-size: 1rem;
      font-weight: 600;
      color: #ffffff;
    }
    .status-badge {
      padding: 0.125rem 0.5rem;
      font-size: 0.75rem;
      font-weight: 500;
      color: #fbbf24;
      background-color: rgba(251, 191, 36, 0.1);
      border: 1px solid rgba(251, 191, 36, 0.3);
      border-radius: 9999px;
    }
    .top-bar-right {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }
    .btn-save {
      padding: 0.4rem 1rem;
      background-color: #2563eb;
      color: #fff;
      border: none;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.8125rem;
      font-weight: 500;
      transition: background-color 0.15s ease;
    }
    .btn-save:hover { background-color: #1d4ed8; }
    .btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
    .btn-submit {
      padding: 0.4rem 1rem;
      background-color: #374151;
      color: #d1d5db;
      border: 1px solid #4b5563;
      border-radius: 0.375rem;
      cursor: not-allowed;
      font-size: 0.8125rem;
      font-weight: 500;
      opacity: 0.6;
    }
    .cancel-link {
      color: #9ca3af;
      font-size: 0.8125rem;
      text-decoration: none;
      cursor: pointer;
    }
    .cancel-link:hover { color: #ffffff; }

    /* Title Section */
    .title-section {
      padding: 1rem 1.5rem;
      background-color: #1a1d2e;
      border-bottom: 1px solid #2a2d3e;
    }
    .field-label {
      display: block;
      margin-bottom: 0.375rem;
      font-weight: 500;
      font-size: 0.875rem;
      color: #d1d5db;
    }
    .title-input {
      width: 100%;
      padding: 0.625rem 0.75rem;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      box-sizing: border-box;
      background-color: #252836;
      color: #ffffff;
      outline: none;
      transition: border-color 0.15s ease;
    }
    .title-input::placeholder { color: #6b7280; }
    .title-input:focus { border-color: #2563eb; }

    /* Mobile Tabs */
    .mobile-tabs {
      display: none;
      padding: 0.5rem 1.5rem;
      background-color: #1a1d2e;
      border-bottom: 1px solid #2a2d3e;
      gap: 0.5rem;
    }
    .tab-btn {
      padding: 0.375rem 1rem;
      background: transparent;
      color: #9ca3af;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.8125rem;
      transition: all 0.15s ease;
    }
    .tab-btn.active {
      background-color: #2563eb;
      color: #fff;
      border-color: #2563eb;
    }

    /* Split Pane */
    .split-pane {
      flex: 1;
      display: flex;
      min-height: 0;
    }
    .editor-pane {
      flex: 1;
      display: flex;
      flex-direction: column;
      border-right: 1px solid #2a2d3e;
    }
    .preview-pane {
      flex: 1;
      display: flex;
      flex-direction: column;
      background-color: #252836;
    }
    .preview-header {
      padding: 0.5rem 1rem;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: #6b7280;
      border-bottom: 1px solid #2a2d3e;
      background-color: #252836;
    }
    .preview-content {
      flex: 1;
      padding: 1rem;
      overflow-y: auto;
      color: #d1d5db;
      font-size: 0.875rem;
      line-height: 1.6;
    }

    /* Toolbar */
    .toolbar {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      padding: 0.375rem 0.75rem;
      background-color: #1a1d2e;
      border-bottom: 1px solid #2a2d3e;
    }
    .toolbar-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 2rem;
      height: 2rem;
      background: transparent;
      color: #9ca3af;
      border: none;
      border-radius: 0.25rem;
      cursor: pointer;
      font-size: 0.8125rem;
      transition: all 0.15s ease;
    }
    .toolbar-btn:hover {
      background-color: #2a2d3e;
      color: #ffffff;
    }

    /* Markdown Editor */
    .markdown-editor {
      flex: 1;
      width: 100%;
      padding: 1rem;
      border: none;
      outline: none;
      resize: none;
      font-family: 'Fira Code', 'Cascadia Code', 'JetBrains Mono', 'Consolas', monospace;
      font-size: 0.875rem;
      line-height: 1.6;
      color: #d1d5db;
      background-color: #1e2030;
      box-sizing: border-box;
    }
    .markdown-editor::placeholder { color: #6b7280; }

    /* Markdown rendered content */
    .markdown-body :first-child { margin-top: 0; }
    .markdown-body h1, .markdown-body h2, .markdown-body h3 {
      color: #ffffff;
      margin-top: 1.5rem;
      margin-bottom: 0.5rem;
    }
    .markdown-body h1 { font-size: 1.5rem; }
    .markdown-body h2 { font-size: 1.25rem; }
    .markdown-body h3 { font-size: 1.1rem; }
    .markdown-body p { margin: 0.5rem 0; }
    .markdown-body code {
      background-color: #1e2030;
      padding: 0.125rem 0.375rem;
      border-radius: 0.25rem;
      font-family: monospace;
      font-size: 0.8125rem;
    }
    .markdown-body pre {
      background-color: #1e2030;
      padding: 0.75rem;
      border-radius: 0.375rem;
      overflow-x: auto;
    }
    .markdown-body pre code {
      background: none;
      padding: 0;
    }
    .markdown-body ul, .markdown-body ol {
      padding-left: 1.5rem;
      margin: 0.5rem 0;
    }
    .markdown-body a { color: #3b82f6; }
    .markdown-body blockquote {
      border-left: 3px solid #3a3f54;
      margin: 0.5rem 0;
      padding-left: 1rem;
      color: #9ca3af;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .mobile-tabs { display: flex; }
      .split-pane { flex-direction: column; }
      .editor-pane { border-right: none; }
      .hidden-mobile { display: none !important; }
      .top-bar {
        flex-direction: column;
        gap: 0.5rem;
        align-items: flex-start;
      }
      .top-bar-right {
        width: 100%;
        justify-content: flex-end;
      }
      .btn-submit { display: none; }
    }
  `]
})
export class AdrEditorComponent implements OnInit {
  @ViewChild('editorTextarea') editorTextarea!: ElementRef<HTMLTextAreaElement>;

  title = '';
  content = '';

  readonly saving = signal(false);
  readonly activeTab = signal<'edit' | 'preview'>('edit');

  readonly renderedHtml = computed(() => {
    try {
      return marked.parse(this.content || '', { async: false }) as string;
    } catch {
      return '';
    }
  });

  constructor(
    private readonly adrService: AdrService,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    const tenant = this.authService.currentTenant();
    if (tenant) {
      this.adrService.getTemplate(tenant.slug).subscribe({
        next: (resp) => {
          this.content = resp.content;
        },
        error: () => {
          this.content = this.defaultTemplate();
        }
      });
    } else {
      this.content = this.defaultTemplate();
    }
  }

  saveDraft(): void {
    if (!this.title.trim()) {
      this.toastService.show('ADR title is required', 'error');
      return;
    }

    const requiredSections = ['## Status', '## Context', '## Decision'];
    const missingSections = requiredSections.filter(s => !this.content.includes(s));
    if (missingSections.length > 0) {
      this.toastService.show(`Missing sections: ${missingSections.join(', ')}`, 'error');
      return;
    }

    const tenant = this.authService.currentTenant();
    if (!tenant) {
      this.toastService.show('No tenant selected', 'error');
      return;
    }

    this.saving.set(true);

    this.adrService.createAdr(tenant.slug, {
      title: this.title,
      content: this.content
    }).subscribe({
      next: (adr) => {
        this.saving.set(false);
        this.toastService.show('ADR draft saved successfully', 'success');
        this.router.navigate(['/adrs', adr.id]);
      },
      error: () => {
        this.saving.set(false);
        this.toastService.show('Failed to save ADR draft', 'error');
      }
    });
  }

  insertMarkdown(type: string): void {
    const textarea = this.editorTextarea?.nativeElement;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selected = this.content.substring(start, end);

    let insertion = '';
    let cursorOffset = 0;

    switch (type) {
      case 'bold':
        insertion = `**${selected || 'bold text'}**`;
        cursorOffset = selected ? insertion.length : 2;
        break;
      case 'italic':
        insertion = `*${selected || 'italic text'}*`;
        cursorOffset = selected ? insertion.length : 1;
        break;
      case 'code':
        insertion = `\`${selected || 'code'}\``;
        cursorOffset = selected ? insertion.length : 1;
        break;
      case 'link':
        insertion = `[${selected || 'link text'}](url)`;
        cursorOffset = selected ? insertion.length : 1;
        break;
      case 'list':
        insertion = `\n- ${selected || 'list item'}`;
        cursorOffset = insertion.length;
        break;
      case 'image':
        insertion = `![${selected || 'alt text'}](image-url)`;
        cursorOffset = selected ? insertion.length : 2;
        break;
    }

    this.content = this.content.substring(0, start) + insertion + this.content.substring(end);

    setTimeout(() => {
      textarea.focus();
      const pos = start + cursorOffset;
      textarea.setSelectionRange(pos, pos);
    });
  }

  private defaultTemplate(): string {
    return `## Status

Proposed

## Context

[Describe the context and problem statement]

## Decision

[Describe the decision that was made]

## Consequences

[Describe the consequences of this decision]
`;
  }
}
