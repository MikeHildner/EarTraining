import { Component, OnInit } from '@angular/core';
import { Hero } from '../hero';
//import { HEROES } from '../mock-heroes';
import { HeroService } from '../hero.service';
import { MessageService } from '../message.service';

@Component({
  selector: 'app-heroes',
  templateUrl: './heroes.component.html',
  styleUrls: ['./heroes.component.css']
})
export class HeroesComponent implements OnInit {

  //selectedHero?: Hero;

  //onSelect(hero: Hero): void {
  //  this.selectedHero = hero;
  //  this.messageService.add(`HeroesComponent: Selected hero id=${hero.id}, name=${hero.name}`);
  //}

  //heroes = HEROES;
  heroes: Hero[] = [];

  //getHeroes(): void {
  //  this.heroes = this.heroService.getHeroes();
  //}

  getHeroes(): void {
    this.heroService.getHeroes()
      .subscribe(heroes => this.heroes = heroes);
  }

  constructor(private heroService: HeroService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.getHeroes();
  }

}
