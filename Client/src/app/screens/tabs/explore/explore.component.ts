import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, ViewChild, inject } from '@angular/core';
import { BoundingSphere, Camera, Cartesian2, Cartesian3, Cartographic, Cesium3DTileset, CesiumWidget, CustomDataSource, DataSourceCollection, DataSourceDisplay, DistanceDisplayCondition, Entity, HeightReference, Ray, Scene, SceneMode, ScreenSpaceEventHandler, ScreenSpaceEventType, createGooglePhotorealistic3DTileset, createWorldTerrainAsync, sampleTerrainMostDetailed } from "@cesium/engine";
import { IonButton, IonSelect, IonSelectOption, IonButtons, IonContent, IonFab, IonFabButton, IonFabList, IonHeader, IonIcon, IonImg, IonItem, IonLabel, IonList, IonModal, IonSearchbar, IonTitle, IonToolbar } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { alertSharp, chevronUpSharp, homeSharp } from 'ionicons/icons';
import { BehaviorSubject, Subject, tap } from 'rxjs';
import { IPlaceFeature } from 'src/app/services/geocode/geocode.model';
import { GeocodeService } from 'src/app/services/geocode/geocode.service';
import { PhotoCaptureService } from '../../../services/photo-capture/photo-capture.service';
import { CreatePropertyFormComponent } from 'src/app/components/create-property-form/create-property-form.component';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';
import { IPropertyDetail, IPropertyDto } from 'src/app/models/DTOs/Property/IPropertyDto';
import { PropertyFormComponent } from 'src/app/components/property-form/property-form.component';
import { CreateTaskFormComponent } from 'src/app/components/create-task-form/create-task-form.component';
import { ITaskDetail } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { TaskFormComponent } from 'src/app/components/task-form/task-form.component';

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
    IonSelect,
    IonSelectOption,
    CreatePropertyFormComponent,
    CreateTaskFormComponent,
    PropertyFormComponent,
    TaskFormComponent
  ],
})
export class ExploreComponent implements AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;
  @ViewChild("propertySelectionContainer") propertySelection!: IonSelect

  
  private _photoCaptureService = inject(PhotoCaptureService);
  private _propertyFacadeService = inject(PropertyFacadeService);
  private _dataSourceDisplay!: DataSourceDisplay;
  private _scene!: Scene;
  private _cesiumWidget!: CesiumWidget;
  private _dataCollectionSource = new DataSourceCollection();
  private _dataSource = new CustomDataSource("Custom data source");

  private _geocoder = inject(GeocodeService);

  private googleBuildings!: Cesium3DTileset;
  protected currentPhoto?: string;
  protected searchValue?: string;
  private _searchResults$ = new Subject<IPlaceFeature[] | null>();
  protected searchResults$ = this._searchResults$.asObservable();

  private _selectedPosition$ = new Subject<Cartographic | null>();
  protected selectedPosition$ = this._selectedPosition$.asObservable();

  private _pendingPropertySelection$ = new BehaviorSubject<boolean>(false);
  protected pendingPropertySelection$ = this._pendingPropertySelection$.asObservable();

  private _pendingTaskSelection$ = new BehaviorSubject<boolean>(false);
  protected pendingTaskSelection$ = this._pendingTaskSelection$.asObservable();

  protected propertyCollection$ = this._propertyFacadeService.propertyCollection$;

  private _selectedPropertyUpdated$ = new Subject<IPropertyDetail>();
  protected selectedPropertyUpdated$ = this._selectedPropertyUpdated$.asObservable();

  private _propertyFormDetailsSelected$ = new Subject<IPropertyDetail>();
  protected propertyFormDetailsSelected$ = this._propertyFormDetailsSelected$.asObservable();

  private _taskFormDetailsSelected$ = new Subject<ITaskDetail>();
  protected taskFormDetailsSelected$ = this._taskFormDetailsSelected$.asObservable();

  constructor() {
    addIcons({homeSharp, alertSharp, chevronUpSharp});
  }

  async ngAfterViewInit() {

    this._cesiumWidget = new CesiumWidget("cesiumContainer", {
      terrainProvider: await createWorldTerrainAsync(),
      scene3DOnly: true,
      sceneMode: SceneMode.SCENE3D
    });


    this.styleContainer(this._cesiumWidget);
    
    await this.addGoogleBuildings(this._cesiumWidget.scene);

    this.cameraChangedEvent(this._cesiumWidget.camera);

    this.clickEvent(this._cesiumWidget.scene);

    this._scene = this._cesiumWidget.scene

    this._dataSourceDisplay = new DataSourceDisplay({
      scene: this._scene,
      dataSourceCollection: this._dataCollectionSource
    });
    this._dataCollectionSource.add(this._dataSource);

    this.listenToPropertyChanges();
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

  private listenToPropertyChanges() {

    this._cesiumWidget.clock.onTick.addEventListener(() =>  {
      this._dataSourceDisplay.update(this._cesiumWidget.clock.currentTime);
    });

    this._propertyFacadeService.propertyCollection$
      .subscribe(res => {
          res.forEach(property => {

            if (!this._dataSource.entities.getById(property.property.id)) {

              const long = property.location.longitude;
              const lat = property.location.latitude;
              const height = property.location.elevation;

              const entity = {
                id: property.property.id,
                position: Cartesian3.fromDegrees(long, lat, height),
                billboard: {
                  image: './assets/billboard_icons/Site_Pin.png',
                  heightReference: HeightReference.NONE,
                  pixelOffset: new Cartesian2(0, -5)
                },
                properties: { 
                  type: 'property',
                  ownerId: property.property.ownerId
                }
              };
              this._dataSource.entities.add(entity);
            }
          });
        });

    this.selectedPropertyUpdated$
      .subscribe(property => {
        property.taskAssignments.forEach((task) => {
          if (!this._dataSource.entities.getById(task.id) && task.location) {

            const long = task.location.longitude;
            const lat = task.location.latitude;
            const height = task.location.elevation;
  
            const entity = {
              id: task.id,
              position: Cartesian3.fromDegrees(long, lat, height),
              billboard: {
                image: './assets/billboard_icons/Feature_Pin.png',
                heightReference: HeightReference.NONE,
                pixelOffset: new Cartesian2(0, -5),
                distanceDisplayCondition: new DistanceDisplayCondition(0, 600.0)
              },
              properties: { 
                type: 'task',
                propertyId: property.id,
                ownerId: property.ownerId.id
              }
            };

            this._dataSource.entities.add(entity);
          }
        });
      });
  }

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
        const sphere = new BoundingSphere(Cartographic.toCartesian(carto), 250);
        this._scene.camera.flyToBoundingSphere(sphere);
      });
  }


  protected takePicture() {
    this._photoCaptureService.addNewToGallery();
  }

  protected newPropertySelected() {
    this._pendingPropertySelection$.next(true);
  }

  protected newTaskSelected() {
    this._pendingTaskSelection$.next(true);
  }

  protected handleAddTaskCancelled() {
    this._pendingTaskSelection$.next(false);
    this._selectedPosition$.next(null);
  }

  protected handleTaskAdded(updatedProperty: IPropertyDetail) {
    this._pendingTaskSelection$.next(false);
    this._selectedPosition$.next(null);
    this._selectedPropertyUpdated$.next({...updatedProperty});
  }

  protected handleAddPropertyCancelled() {
    this._pendingPropertySelection$.next(false);
    this._selectedPosition$.next(null);
  }

  protected handlePropertyAdded(propertyDto: IPropertyDto) {
    this._pendingPropertySelection$.next(false);
    this._selectedPosition$.next(null);
    const property = this._propertyFacadeService.getPropertyAssignment(propertyDto.id);
    if (property) {
      this.propertySelection.value = property;
    }
  }

  protected cancel() {
    this.modal.dismiss(null, 'cancel');
    this.modal.isOpen = false;
  }

  protected confirm() {
    this.modal.dismiss(null, 'confirm');
    this.modal.isOpen = false;
  }

  protected handlePropertySelection(event: CustomEvent) {
    const property = event.detail.value as IPropertyAssignment;
    
    const location = property.location;
    const position = Cartesian3.fromDegrees(location.longitude, location.latitude, location.elevation);
    const sphere = new BoundingSphere(position, 250);
    this._scene.camera.flyToBoundingSphere(sphere);
    this._propertyFacadeService.getProperty(property.property.id, property.property.ownerId)
      .subscribe(res => this._selectedPropertyUpdated$.next({...res}));
  }

  private clickEvent(scene: Scene) {
    const handler = new ScreenSpaceEventHandler(scene.canvas);

    handler.setInputAction((event: ScreenSpaceEventHandler.PositionedEvent) => {
  
      const pickResult = scene.pick(event.position, 3, 3);

      if (pickResult?.id instanceof Entity) {

        const entity = pickResult.id;

        if (entity.properties.type.getValue(this._cesiumWidget.clock.currentTime) === "property") {
          this._propertyFacadeService.getProperty(entity.id, entity.properties.ownerId.getValue(this._cesiumWidget.clock.currentTime))
          .subscribe(res => {
            const property = this._propertyFacadeService.getPropertyAssignment(pickResult.id.id);
            this._propertyFormDetailsSelected$.next({...res});
            if (property) {
              this.propertySelection.value = property;
            }
          });
        }

        if (entity.properties.type.getValue(this._cesiumWidget.clock.currentTime) === "task") {
          this._propertyFacadeService.getTask(
            entity.id, 
            entity.properties.propertyId.getValue(this._cesiumWidget.clock.currentTime), 
            entity.properties.ownerId.getValue(this._cesiumWidget.clock.currentTime))
          .subscribe(res => {
            this._taskFormDetailsSelected$.next({...res});
          });
        }

        return;
      }

      if (this._pendingPropertySelection$.getValue() || this._pendingTaskSelection$.getValue()) {

        const position = this.getWorldPickPosition(scene, event.position);

        if (position && !position.equals(Cartesian3.ZERO)) {
          // this._pendingPropertySelection$.next(false);
          const carto = Cartographic.fromCartesian(position);
          this._selectedPosition$.next(carto);
        }
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

/**
 * Gets the Cartesian3 world position at the input mouse position at either the
 * @param {Scene} scene The scene
 * @param {Cartesian2} mousePosition The mouse position
 * @returns {Cartesian3} The position in world space
 */
 private getWorldPickPosition(scene: Scene, mousePosition: Cartesian2): Cartesian3 | undefined {
    const _rayScratch = new Ray();
    let position: Cartesian3 | undefined;
    scene.camera.getPickRay(mousePosition, _rayScratch);
    if (scene.pickPositionSupported) {
        // Don't pick default 3x3, or scene.pick may allow a mousePosition that isn't on the tileset to pickPosition.
        const pickedObject = scene.pick(mousePosition, 1, 1);
        if (pickedObject) {
            // Get the intersection of the ray and the picked object
            // pickFromRay is a private cesium function
            position = (scene as any).pickFromRay(_rayScratch)?.position;
            // check to let us know if we should pick against the globe instead
            if (position) {
                return Cartesian3.clone(position);
            }
        }
    }
    if (!scene.globe) {
        return;
    }
    // Get the terrain position if no objects have been selected
    position = scene.globe.pick(_rayScratch, scene);
    if (position) {
        return Cartesian3.clone(position);
    }
    return;
  }
}
