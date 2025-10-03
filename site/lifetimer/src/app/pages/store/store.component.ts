import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MainLayoutComponent } from '../../layout/main-layout.component';

@Component({
  selector: 'app-store',
  imports: [MainLayoutComponent, CommonModule, MatButtonModule],
  templateUrl: './store.component.html',
  styleUrl: './store.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class StoreComponent implements OnInit {
  isMobile = false;
  storeUrl = 'https://apps.microsoft.com/detail/9MT8BCKRPSSW';

  ngOnInit() {
    this.checkIfMobile();
  }

  private isMobileDevice(): boolean {
    const userAgent =
      navigator.userAgent || navigator.vendor || (window as any).opera;
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(
      userAgent.toLowerCase()
    );
  }

  private checkIfMobile() {
    this.isMobile = this.isMobileDevice();
    console.log('Check is mobile ' + this.isMobile);

    // Also listen for resize events
    window.addEventListener('resize', () => {
      this.isMobile = window.innerWidth <= 768;
    });
  }

  openStoreLink() {
    window.open(this.storeUrl, '_blank');
  }
}
