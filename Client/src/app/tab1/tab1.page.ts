import { AfterViewInit, Component, ViewChild, inject } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonFab, IonFabButton, IonIcon, IonModal, IonButton, IonButtons, IonImg } from '@ionic/angular/standalone';
import { ExploreContainerComponent } from '../components/explore-container/explore-container.component';
import { Camera, Cartesian2, Cartesian3, Cesium3DTileset, CesiumWidget, CustomDataSource, DataSourceCollection, DataSourceDisplay, Entity, HeightReference, Scene, SceneMode, ScreenSpaceEventHandler, ScreenSpaceEventType, createGooglePhotorealistic3DTileset, createWorldTerrainAsync } from "@cesium/engine";
import { PhotoCaptureService } from './../services/photo-capture/photo-capture.service';
import { camera } from 'ionicons/icons';
import { addIcons } from 'ionicons';


@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss'],
  standalone: true,
  imports: [
    IonHeader, 
    IonToolbar, 
    IonTitle, 
    IonContent, 
    IonFab,
    IonFabButton,
    IonIcon,
    IonModal,
    IonButtons,
    IonButton,
    IonImg,
    ExploreContainerComponent],
})

export class Tab1Page implements AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;

  
  private _photoCaptureService = inject(PhotoCaptureService);
  private _dataSourceDisplay!: DataSourceDisplay;
  private _dataCollectionSource = new DataSourceCollection();
  private _dataSource = new CustomDataSource("Custom data source");

  private googleBuildings!: Cesium3DTileset;

  protected currentPhoto?: string;

  constructor() {
    addIcons({camera});
  }

  async ngAfterViewInit() {

    const cesiumWidget = new CesiumWidget("cesiumContainer", {
      terrainProvider: await createWorldTerrainAsync(),
      scene3DOnly: true,
      sceneMode: SceneMode.SCENE3D
    });

    await this.addGoogleBuildings(cesiumWidget.scene);

    this.cameraChangedEvent(cesiumWidget.camera);

    this.clickEvent(cesiumWidget.scene);

    this._dataSourceDisplay = new DataSourceDisplay({
      scene: cesiumWidget.scene,
      dataSourceCollection: this._dataCollectionSource
    });
    this._dataCollectionSource.add(this._dataSource);

    this._photoCaptureService.nextPhoto$.subscribe(res => {
      if (res.location) {

        const long = res.location.coords.longitude;
        const lat = res.location.coords.latitude;

        const entity = {
          id: res.id,
          position: Cartesian3.fromDegrees(long, lat),
          billboard: {
            image: './assets/billboard_icons/Feature_Pin.png',
            heightReference: HeightReference.CLAMP_TO_GROUND,
            pixelOffset: new Cartesian2(0, -5)
          }
        };

        const added = this._dataSource.entities.add(entity);
        this._dataSourceDisplay.update(cesiumWidget.clock.currentTime);
      }
    })

  }

  // const long = res.location.coords.longitude + (0.1 * Math.random());
  // const lat = res.location.coords.latitude + (0.1 * Math.random());

  private async addGoogleBuildings(scene: Scene) {
    this.googleBuildings = await createGooglePhotorealistic3DTileset(undefined, {
      skipLevelOfDetail: true,
      enableCollision: false,
      show: false,
      preloadWhenHidden: true,
    });

    scene.primitives.add(this.googleBuildings);
  }

  protected takePicture() {
    this._photoCaptureService.addNewToGallery();
  }

  cancel() {
    this.modal.dismiss(null, 'cancel');
    this.modal.isOpen = false;
  }

  confirm() {
    this.modal.dismiss(null, 'confirm');
    this.modal.isOpen = false;
  }

  private clickEvent(scene: Scene) {
    const handler = new ScreenSpaceEventHandler(scene.canvas);

    handler.setInputAction((event: ScreenSpaceEventHandler.PositionedEvent) => {
  
      const pickResult = scene.pick(event.position, 3, 3);

      if (pickResult?.id instanceof Entity) {
        const userPhoto = this._photoCaptureService.getPhotoById(pickResult.id.id);
        this.currentPhoto = userPhoto?.webviewPath;
        this.modal.isOpen = true;
      }

      
    }, ScreenSpaceEventType.LEFT_CLICK)
    

  }

  private cameraChangedEvent(camera: Camera) {
    camera.changed.addEventListener(res => {

      if (camera.positionCartographic.height < 5000) {
        this.googleBuildings.show = true;
      } else {
        this.googleBuildings.show = false;
      }

    })
  }
}
