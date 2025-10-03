import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-docs-layout',
  imports: [CommonModule, MatButtonModule],
  templateUrl: './docs-layout.component.html',
  styleUrl: './docs-layout.component.scss',
})
export class DocsLayoutComponent {
  isSidebarOpen = signal(false);

  constructor(private navigationService: NavigationService) {
    // Debug logging
    console.log('DocsLayoutComponent initialized');
  }

  get isAppNavOpen() {
    return this.navigationService.mobileMenuOpen;
  }

  isTocDisabled = computed(() => {
    const disabled = this.isAppNavOpen();
    console.log('TOC disabled:', disabled);
    return disabled;
  });

  sidebarShouldBeOpen = computed(() => this.isSidebarOpen() && !this.isTocDisabled());

  toggleSidebar() {
    if (!this.isTocDisabled()) {
      this.isSidebarOpen.update(open => !open);
    }
  }
}