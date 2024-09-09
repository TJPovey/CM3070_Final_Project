import { DatePipe } from '@angular/common';
import { inject, Injectable } from '@angular/core';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable'
import { IPropertyDetail } from 'src/app/models/DTOs/Property/IPropertyDto';
import { ITaskDetail } from 'src/app/models/DTOs/Tasks/ITaskDto';

@Injectable({
  providedIn: 'root'
})
export class PdfGeneratorService {

  private _datePipe = inject(DatePipe);

  generateReport(
    property: IPropertyDetail, 
    propertyImage: Uint8Array, 
    tasks: ITaskDetail[], 
    taskImages: Map<string, Uint8Array>) {

    const doc = new jsPDF('p', 'mm', 'a4');
    const pageWidth = doc.internal.pageSize.getWidth();

    // Header
    doc.setFontSize(18);
    doc.setFont('helvetica', 'bold');
    doc.text('SnagIt', pageWidth / 2, 15, { align: 'center' });

    // Report Title
    doc.setFontSize(16);
    doc.setFont('helvetica', 'bold');
    doc.text(property.reportTitle, pageWidth / 2, 30, { align: 'center' });

    // Property Name
    doc.setFontSize(14);
    doc.setFont('helvetica', 'normal');
    doc.text(property.propertyName, pageWidth / 2, 40, { align: 'center' });

    if (propertyImage) {
      const imgProps = doc.getImageProperties(propertyImage);
      const imgHeight = (imgProps.height * pageWidth) / imgProps.width;
      doc.addImage(propertyImage, 'png', 10, 50, pageWidth - 20, imgHeight);
    }

    let currentY = 60 + (propertyImage ? (doc.getImageProperties(propertyImage).height * (pageWidth - 20)) / doc.getImageProperties(propertyImage).width : 0) + 10;

    currentY += 20;

    // User Assignments Table
    doc.setFontSize(14);
    doc.text('User Assignments', 10, currentY);

    const userAssignments = property.userAssignments.map(user => [
      user.userName,
      user.role.name,
    ]);

    autoTable(doc, {
      head: [['User Name', 'Role']],
      body: userAssignments,
      startY: currentY + 5,
      theme: 'striped',
      styles: { fontSize: 12 },
    });
    
    
    // Iterate over each task and create a page for it
    tasks.forEach((task) => {

      doc.addPage();
      currentY = 0;

      doc.setFontSize(14);
      doc.setFont('helvetica', 'bold');
      doc.text(task.title, pageWidth / 2, 15, { align: 'center' });
      
      currentY += 20;

      // Task Details
      doc.setFontSize(10);
      doc.setFont('helvetica', 'normal');
      doc.text(`Status: ${task.open ? 'Open' : 'Closed'}`, 10, currentY);
      currentY += 6;
      doc.text(`Area: ${task.area}`, 10, currentY);
      currentY += 6;
      doc.text(`Description: ${task.description}`, 10, currentY);
      currentY += 6;
      doc.text(`Due Date: ${this._datePipe.transform(task.dueDate)}`, 10, currentY);
      currentY += 6;
      doc.text(`Estimated Cost: £${task.estimatedCost.toFixed(2)}`, 10, currentY);
      currentY += 6;
      doc.text(`Category: ${task.category.name}`, 10, currentY);
      currentY += 6;
      doc.text(`Priority: ${task.priority.name}`, 10, currentY);
      currentY += 6;
      doc.text(`Assigned To: ${task.assignedUser.userName}`, 10, currentY);
      currentY += 10;

      if (taskImages.has(task.id)) {
        const taskImage = taskImages.get(task.id)!;
        const taskImgProps = doc.getImageProperties(taskImage);
        const taskImgWidth = pageWidth - 40; // Adjust the width as necessary
        const taskImgHeight = (taskImgProps.height * taskImgWidth) / taskImgProps.width;
        const imgProps = doc.getImageProperties(propertyImage);
        const imgHeight = (imgProps.height * pageWidth) / imgProps.width;
        doc.addImage(taskImage, 'png', 10, currentY, pageWidth - 20, imgHeight);
        currentY += taskImgHeight + 10;
      }

    });

    // Save the PDF
    doc.save(`${property.reportTitle}_SnagIt_Report.pdf`);
  }
}
