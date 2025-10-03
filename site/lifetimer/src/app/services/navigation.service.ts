import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  private mobileMenuOpenSignal = signal(false);

  get mobileMenuOpen() {
    return this.mobileMenuOpenSignal.asReadonly();
  }

  setMobileMenuOpen(isOpen: boolean): void {
    this.mobileMenuOpenSignal.set(isOpen);
  }
}