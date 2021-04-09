import { Component, OnInit } from '@angular/core';
import { Track } from 'ngx-audio-player'; 

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

  constructor() { }

  ngOnInit(): void {
  }

}
