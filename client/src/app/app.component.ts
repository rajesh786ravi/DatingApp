import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, FormsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  showLoader = false;
  http = inject(HttpClient);
  title = 'Rajesh';
  users: any;
  localPath: string = '';

  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed.')
    })
  }

  openLink(link: string) {
    this.showLoader = true;

    setTimeout(() => {
      window.location.href = link;
    }, 5000); // 5 second delay
  }

  savePath(): void {
    this.http.post('https://localhost:5001/api/drive/save-path', { localPath: this.localPath }).subscribe({
      next: (res) => alert('Path saved!'),
      error: (err) => alert('Failed to save path...')
    });
  }

  onFolderSelected(event: any) {
    const files: FileList = event.target.files;
    debugger
    if (files.length > 0) {
      // Get folder path from first file
      const fullPath = (files[0] as any).webkitRelativePath;
      const folder = fullPath.split('/')[0]; // extract folder name
      this.localPath = folder;
    }
  }

  updateDrive(): void {
    this.http.post('https://localhost:5001/api/drive/update-drive', {}).subscribe({
      next: (res: any) => alert(res.message || 'Updated!'),
      error: () => alert('Update failed.')
    });
  }
}
