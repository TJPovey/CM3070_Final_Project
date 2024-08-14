import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, ViewChild, inject } from '@angular/core';
import { BoundingSphere, Camera, Cartesian2, Cartesian3, Cartographic, Cesium3DTileset, CesiumWidget, CustomDataSource, DataSourceCollection, DataSourceDisplay, Entity, HeightReference, Scene, SceneMode, ScreenSpaceEventHandler, ScreenSpaceEventType, createGooglePhotorealistic3DTileset, createWorldTerrainAsync, sampleTerrainMostDetailed } from "@cesium/engine";
import { IonButton, IonButtons, IonContent, IonFab, IonFabButton, IonFabList, IonHeader, IonIcon, IonImg, IonItem, IonLabel, IonList, IonModal, IonSearchbar, IonTitle, IonToolbar } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { alertSharp, chevronUpSharp, homeSharp } from 'ionicons/icons';
import { Subject } from 'rxjs';
import { IPlaceFeature } from 'src/app/services/geocode/geocode.model';
import { GeocodeService } from 'src/app/services/geocode/geocode.service';
import { ExploreContainerComponent } from '../../../components/explore-container/explore-container.component';
import { PhotoCaptureService } from '../../../services/photo-capture/photo-capture.service';

@Component({
  selector: 'app-explore',
  templateUrl: './explore.component.html',
  styleUrls: ['./explore.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    IonSearchbar,
    IonHeader, 
    IonToolbar, 
    IonTitle, 
    IonList,
    IonItem,
    IonLabel,
    IonContent, 
    IonFab,
    IonFabButton,
    IonFabList,
    IonIcon,
    IonModal,
    IonButtons,
    IonButton,
    IonImg,
    ExploreContainerComponent],
})
export class ExploreComponent implements AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;

  
  private _photoCaptureService = inject(PhotoCaptureService);
  private _dataSourceDisplay!: DataSourceDisplay;
  private _scene!: Scene;
  private _dataCollectionSource = new DataSourceCollection();
  private _dataSource = new CustomDataSource("Custom data source");

  private _geocoder = inject(GeocodeService);

  private googleBuildings!: Cesium3DTileset;

  protected currentPhoto?: string;
  protected searchValue?: string;

  private _searchResults$ = new Subject<IPlaceFeature[] | null>();
  protected searchResults$ = this._searchResults$.asObservable();

  constructor() {
    addIcons({homeSharp, alertSharp, chevronUpSharp});
  }

  async ngAfterViewInit() {

    const cesiumWidget = new CesiumWidget("cesiumContainer", {
      terrainProvider: await createWorldTerrainAsync(),
      scene3DOnly: true,
      sceneMode: SceneMode.SCENE3D
    });


    this.styleContainer(cesiumWidget);
    
    await this.addGoogleBuildings(cesiumWidget.scene);

    this.cameraChangedEvent(cesiumWidget.camera);

    this.clickEvent(cesiumWidget.scene);

    this._scene = cesiumWidget.scene

    this._dataSourceDisplay = new DataSourceDisplay({
      scene: this._scene,
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

  private styleContainer(widget: CesiumWidget) {
    const container = ((widget as any)._creditContainer as HTMLElement)
      .getElementsByClassName('cesium-widget-credits')[0] as HTMLElement;
    container.style.padding = "0.5rem";
    const logo = container.getElementsByClassName('cesium-credit-logoContainer')[0] as HTMLElement;
    logo.style.display = "none";
    const links = container.getElementsByClassName('cesium-credit-expand-link')[0] as HTMLElement;
    links.remove();
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

  protected searchChanged(event: CustomEvent) {
    if (event.detail.value) {
      this._geocoder.searchPlace(event.detail.value)
        .subscribe(res => this._searchResults$.next(res));
    }
  }

  protected searchCleared() {
    this._searchResults$.next(null)
  }

  protected goToDestination(place: IPlaceFeature, searchbar: IonSearchbar) {
    this.searchCleared();
    searchbar.value = "";
    const carto = Cartographic.fromDegrees(place.longitude, place.latitude);

    sampleTerrainMostDetailed(this._scene.terrainProvider, [carto])
      .then((updatedPositions) => {
        const carto = Cartographic.clone(updatedPositions[0])
        const sphere = new BoundingSphere(Cartographic.toCartesian(carto), 250)
        this._scene.camera.flyToBoundingSphere(sphere);
      });
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
