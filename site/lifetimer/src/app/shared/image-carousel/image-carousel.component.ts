import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface CarouselImage {
  src: string;
  alt: string;
  caption?: string;
}

@Component({
  selector: 'app-image-carousel',
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './image-carousel.component.html',
  styleUrl: './image-carousel.component.scss'
})
export class ImageCarouselComponent implements OnInit, OnDestroy {
  @Input() images: CarouselImage[] = [];
  @Input() autoplay = true;
  @Input() autoplayInterval = 5000;
  @Input() showIndicators = true;
  @Input() showNavigation = true;

  currentIndex = 0;
  private autoplayTimer: any;

  ngOnInit() {
    if (this.autoplay && this.images.length > 1) {
      this.startAutoplay();
    }
  }

  ngOnDestroy() {
    this.stopAutoplay();
  }

  nextSlide() {
    this.currentIndex = (this.currentIndex + 1) % this.images.length;
  }

  previousSlide() {
    this.currentIndex = this.currentIndex === 0 ? this.images.length - 1 : this.currentIndex - 1;
  }

  goToSlide(index: number) {
    this.currentIndex = index;
  }

  openImageInNewTab(imageSrc: string) {
    window.open(imageSrc, '_blank');
  }

  private startAutoplay() {
    this.autoplayTimer = setInterval(() => {
      this.nextSlide();
    }, this.autoplayInterval);
  }

  private stopAutoplay() {
    if (this.autoplayTimer) {
      clearInterval(this.autoplayTimer);
    }
  }

  onMouseEnter() {
    if (this.autoplay) {
      this.stopAutoplay();
    }
  }

  onMouseLeave() {
    if (this.autoplay && this.images.length > 1) {
      this.startAutoplay();
    }
  }
}