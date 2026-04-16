import { Component, signal, computed, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { marked } from 'marked';
import DOMPurify from 'dompurify';
import { AdrService } from '../../../core/services/adr.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../shared/components/toast/toast.service';
import { AdrDetailResponse } from '../../../core/models/adr.model';

@Component({
  selector: 'app-adr-editor',
  standalone: true,
  imports: [FormsModule, RouterLink, NgClass],
  templateUrl: './adr-editor.component.html',
  styleUrl: './adr-editor.component.scss'
})
export class AdrEditorComponent implements OnInit {
  @ViewChild('editorTextarea') editorTextarea!: ElementRef<HTMLTextAreaElement>;

  title = '';
  readonly contentSignal = signal('');

  get content(): string { return this.contentSignal(); }
  set content(value: string) { this.contentSignal.set(value); }

  readonly saving = signal(false);
  readonly activeTab = signal<'edit' | 'preview'>('edit');
  readonly editMode = signal(false);
  readonly existingAdr = signal<AdrDetailResponse | null>(null);
  readonly readOnly = computed(() => {
    const adr = this.existingAdr();
    if (!adr) return false;
    const terminalStatuses = ['accepted', 'rejected', 'superseded', 'deprecated'];
    return terminalStatuses.includes(adr.status);
  });

  getStatusClass(status: string): string {
    const normalized = status.toLowerCase().replace(/\s+/g, '-');
    switch (normalized) {
      case 'draft': return 'status-draft';
      case 'in-review': return 'status-in-review';
      case 'approved': return 'status-approved';
      case 'rejected': return 'status-rejected';
      case 'superseded': return 'status-superseded';
      default: return 'status-draft';
    }
  }

  readonly renderedHtml = computed(() => {
    try {
      const raw = marked.parse(this.contentSignal() || '', { async: false }) as string;
      return DOMPurify.sanitize(raw);
    } catch {
      return '';
    }
  });

  constructor(
    private readonly adrService: AdrService,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const adrId = this.route.snapshot.paramMap.get('id');
    const tenant = this.authService.currentTenant();

    if (adrId && tenant) {
      // Edit mode — load existing ADR
      this.editMode.set(true);
      this.adrService.getAdr(tenant.slug, adrId).subscribe({
        next: (adr) => {
          this.existingAdr.set(adr);
          this.title = adr.title;
          this.content = adr.body;
        },
        error: () => {
          this.toastService.show('Failed to load ADR', 'error');
          this.router.navigate(['/adrs']);
        }
      });
    } else if (tenant) {
      // Create mode — load template
      this.adrService.getTemplate(tenant.slug).subscribe({
        next: (resp) => {
          this.content = resp.body;
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
    if (this.readOnly()) return;

    if (!this.title.trim()) {
      this.toastService.show('ADR title is required', 'error');
      return;
    }

    const requiredSections = ['## Status', '## Context', '## Decision', '## Consequences'];
    const missingSections = requiredSections.filter(s => !this.content.includes(s));
    if (missingSections.length > 0) {
      this.toastService.show(`Warning: Missing sections: ${missingSections.join(', ')}`, 'error');
    }

    const tenant = this.authService.currentTenant();
    if (!tenant) {
      this.toastService.show('No tenant selected', 'error');
      return;
    }

    this.saving.set(true);

    if (this.editMode() && this.existingAdr()) {
      const adr = this.existingAdr()!;
      this.adrService.updateAdr(tenant.slug, adr.id, {
        title: this.title,
        body: this.content,
        tags: adr.tags
      }).subscribe({
        next: (resp) => {
          this.saving.set(false);
          this.toastService.show('ADR updated successfully', 'success');
          this.router.navigate(['/adrs', resp.id, 'edit']);
        },
        error: () => {
          this.saving.set(false);
          this.toastService.show('Failed to update ADR', 'error');
        }
      });
    } else {
      this.adrService.createAdr(tenant.slug, {
        title: this.title,
        body: this.content
      }).subscribe({
        next: (adr) => {
          this.saving.set(false);
          this.toastService.show('ADR draft saved successfully', 'success');
          this.router.navigate(['/adrs', adr.id, 'edit']);
        },
        error: () => {
          this.saving.set(false);
          this.toastService.show('Failed to save ADR draft', 'error');
        }
      });
    }
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
      case 'heading':
        insertion = `\n## ${selected || 'Heading'}`;
        cursorOffset = insertion.length;
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
