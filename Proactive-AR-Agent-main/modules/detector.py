from ultralytics import YOLO

def process_video_frame(video_path):
    model = YOLO("yolov8n.pt")
    results = model.predict(source=video_path, stream=True)
    for result in results:
        boxes = result.boxes
        frame_objects = []
        frame_detections = []  # 用于追踪的详细信息
        
        if boxes is not None:
            for box in boxes:
                cls_id = int(box.cls[0])
                cls_name = result.names[cls_id]
                conf = float(box.conf[0])
                bbox = box.xyxy[0].cpu().numpy()  # [x1, y1, x2, y2]
                
                frame_objects.append(cls_name)
                frame_detections.append({
                    'class': cls_name,
                    'bbox': bbox.tolist(),
                    'conf': conf
                })
        
        yield frame_objects, frame_detections
