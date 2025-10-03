import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MarkdownComponent } from 'ngx-markdown';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MarkdownService } from '../../../services/markdown.service';

@Component({
  selector: 'app-toc',
  imports: [MarkdownComponent, CommonModule, MatCardModule],
  templateUrl: './toc.component.html',
  styleUrl: './toc.component.scss',
})
export class TocComponent implements OnInit {
  @Output() tocItemClicked = new EventEmitter<void>();

  tocContent = '';

  constructor(
    private markdownService: MarkdownService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadToc();
  }

  private loadToc() {
    this.markdownService.loadMarkdown('/docs/toc.md').subscribe((content) => {
      this.tocContent = content;
    });
  }

  onTocClick(event: Event) {
    const target = event.target as HTMLAnchorElement;
    if (target.tagName === 'A' && target.hash) {
      event.preventDefault();
      event.stopPropagation(); // Prevent event bubbling

      // Add touch action handling for mobile
      if ('ontouchstart' in window) {
        event.stopImmediatePropagation();
      }

      const anchor = target.hash.substring(1);
      this.router.navigate(['/docs/document'], { fragment: anchor });

      // Emit event to notify parent that TOC item was clicked
      this.tocItemClicked.emit();
    }
  }
}