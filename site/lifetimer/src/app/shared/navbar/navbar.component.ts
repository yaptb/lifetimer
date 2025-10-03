import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { NavigationService } from '../../services/navigation.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, CommonModule, MatSelectModule, MatFormFieldModule, MatOptionModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  mobileMenuOpen = false;
  selectedLanguage = 'en';

  constructor(private navigationService: NavigationService) {}

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
    console.log('Navbar mobile menu toggled:', this.mobileMenuOpen);
    this.navigationService.setMobileMenuOpen(this.mobileMenuOpen);
  }

  closeMobileMenu() {
    this.mobileMenuOpen = false;
    console.log('Navbar mobile menu closed:', this.mobileMenuOpen);
    this.navigationService.setMobileMenuOpen(this.mobileMenuOpen);
  }

  onLanguageChange(event: any) {
    this.selectedLanguage = event.value;
  }
}
