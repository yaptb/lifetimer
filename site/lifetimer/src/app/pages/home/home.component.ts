import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MainLayoutComponent } from '../../layout/main-layout.component';
import {
  ImageCarouselComponent,
  CarouselImage,
} from '../../shared/image-carousel/image-carousel.component';

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    MatButtonModule,
    MainLayoutComponent,
    ImageCarouselComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  carouselImages: CarouselImage[] = [
    {
      src: '/images/home/screenshot2.png',
      alt: 'LifeTimer desktop clock',
      caption: 'Highly configurable desktop clock',
    },
    {
      src: '/images/home/screenshot3.png',
      alt: 'LifeTimer timer definitions',
      caption: 'Extensive timer settings',
    },
    {
      src: '/images/home/screenshot4.png',
      alt: 'LifeTimer settings and configuration',
      caption: 'Blends perfectly with your desktop',
    },

    {
      src: '/images/home/screenshot1.png',
      alt: 'LifeTimer dashboard display',
      caption: 'Tracks multiple events',
    },
  ];
}
