import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MarkdownComponent } from 'ngx-markdown';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MarkdownService } from '../../../services/markdown.service';

@Component({
  selector: 'app-document-content',
  imports: [MarkdownComponent, CommonModule],
  templateUrl: './document-content.component.html',
  styleUrl: './document-content.component.scss',
})
export class DocumentContentComponent implements OnInit {
  documentContent = '';
  @ViewChild('documentContainer') container!: ElementRef;

  constructor(
    private markdownService: MarkdownService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.loadDocument();
    
    // Watch for fragment changes
    this.route.fragment.subscribe(fragment => {
      if (fragment) {
        this.scrollToAnchor(fragment);
      }
    });
  }

  private loadDocument() {
    this.markdownService.loadDocumentation().subscribe((content) => {
      this.documentContent = content;
      // Try to scroll to fragment after content loads
      setTimeout(() => {
        const fragment = this.route.snapshot.fragment;
        if (fragment) {
          this.scrollToAnchor(fragment);
        }
      }, 200);
    });
  }

  scrollToAnchor(anchor: string) {
    setTimeout(() => {
      const element = document.querySelector(`#${anchor}`);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 100);
  }
}