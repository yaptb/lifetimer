import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { TocComponent } from './components/toc.component';
import { DocsLayoutComponent } from '../../layout/docs-layout.component';

@Component({
  selector: 'app-docs',
  imports: [CommonModule, RouterOutlet, TocComponent, DocsLayoutComponent],
  templateUrl: './docs.component.html',
  styleUrl: './docs.component.scss',
  preserveWhitespaces: true,
})
export class DocsComponent {
  @ViewChild(DocsLayoutComponent) docsLayout!: DocsLayoutComponent;

  onTocItemClicked() {
    // Close the sidebar when a TOC item is clicked (only on mobile)
    if (this.docsLayout) {
      this.docsLayout.isSidebarOpen.set(false);
    }
  }
}
