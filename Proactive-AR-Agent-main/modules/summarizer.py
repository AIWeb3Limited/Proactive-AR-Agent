
from collections import Counter

def update_window(object_window, frame_objects):
    if len(object_window) >= 60:
        object_window.pop(0)
    object_window.append(frame_objects)

def get_attention_summary(object_window):
    flat = [obj for frame in object_window for obj in frame]
    counter = Counter(flat)
    summary = ', '.join(f"{v} 个 {k}" for k, v in counter.items() if v >= 3)
    return summary, counter
