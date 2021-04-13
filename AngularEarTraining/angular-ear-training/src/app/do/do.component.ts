import { Component, OnInit } from '@angular/core';
import { Track } from 'ngx-audio-player';
import { map } from 'rxjs/operators';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'app-do',
  templateUrl: './do.component.html',
  styleUrls: ['./do.component.scss']
})
export class DoComponent implements OnInit {

  doUrl = 'http://hildner.org/eartraining/DO/GetDOEx?doNoteName=C%234%2FDb4';
  msaapDisplayTitle = false;
  msaapDisplayPlayList = false;
  msaapPageSizeOptions = [2, 4, 6];
  msaapDisplayVolumeControls = false;
  msaapDisplayRepeatControls = false;
  msaapDisplayArtist = false;
  msaapDisplayDuration = false;
  msaapDisablePositionSlider = true;

  // Material Style Advance Audio Player Playlist
  msaapPlaylist: Track[] = [
    {
      title: 'DO',
      link: this.doUrl,
      artist: 'Audio One Artist',
      //duration: 'Audio One Duration in seconds'
    },
  ];

  onEnded(event: string): void {
    console.log('onEnded event: ' + event);
  }

  cols = 2;
  rows = 2;

  /** Based on the screen size, switch from standard to one column per row */
  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => {
      if (matches) {
        this.rows = 1;
        this.cols = 1;
      }
      else {
        this.rows = 2;
        this.cols = 2;
      }
    })
  );

  constructor(private breakpointObserver: BreakpointObserver) { }

  ngOnInit(): void {
  }

}
