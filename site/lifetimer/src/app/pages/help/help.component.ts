import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MainLayoutComponent } from '../../layout/main-layout.component';

@Component({
  selector: 'app-help',
  imports: [MainLayoutComponent, MatCardModule],
  templateUrl: './help.component.html',
  styleUrl: './help.component.scss'
})
export class HelpComponent {

}
