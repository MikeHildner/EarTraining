import { Component } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
import { MatMenuTrigger } from '@angular/material/menu';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'angular-ear-training';
  //@ViewChild('sidenav') sidenav: MatSidenav;
  //isExpanded = true;
  //showSubmenu = false;
  //isShowing = false;
  //showSubSubMenu = false;

  //mouseenter() {
  //  if (!this.isExpanded) {
  //    this.isShowing = true;
  //  }
  //}

  //mouseleave() {
  //  if (!this.isExpanded) {
  //    this.isShowing = false;
  //  }
  //}

  @ViewChild(MatMenuTrigger) trigger!: MatMenuTrigger;

  someMethod() {
    this.trigger.openMenu();
  }
}
