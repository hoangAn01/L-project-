# Hướng dẫn chỉnh Layer cho Background trong Unity UI

## Vấn đề thường gặp
Khi thêm background vào UI, background có thể hiển thị phía trên các element khác thay vì phía sau.

## Cách giải quyết

### 1. Sử dụng BackgroundLayerManager (Khuyến nghị)

1. **Thêm script BackgroundLayerManager vào scene:**
   - Tạo một GameObject mới trong scene
   - Thêm component `BackgroundLayerManager`
   - Script sẽ tự động tìm và setup background

2. **Cấu hình trong Inspector:**
   - `Background Image`: Kéo background image vào đây (hoặc để trống để tự động tìm)
   - `Background Sorting Order`: Đặt giá trị âm (ví dụ: -1) để background hiển thị phía sau
   - `UI Sorting Order`: Đặt giá trị 0 hoặc dương để UI hiển thị phía trên

3. **Sử dụng Context Menu:**
   - Click chuột phải vào BackgroundLayerManager component
   - Chọn "Setup Background Layer" để setup tự động
   - Chọn "Move Background to Back" để di chuyển background ra sau
   - Chọn "Show Layer Info" để xem thông tin layer

### 2. Sử dụng QuickFix (Tự động)

1. **Thêm script QuickFix vào scene:**
   - Script sẽ tự động fix background layer khi Start()
   - Bật `Fix Background Layer` trong Inspector

2. **Cấu hình:**
   - `Background Sorting Order`: Đặt giá trị âm (ví dụ: -1)

### 3. Cách thủ công trong Unity Editor

#### Cách 1: Sắp xếp trong Hierarchy
1. Chọn background GameObject
2. Kéo background lên vị trí đầu tiên trong hierarchy (trên cùng)
3. Các element sau sẽ hiển thị phía trên background

#### Cách 2: Sử dụng Sorting Order
1. Chọn background GameObject
2. Thêm component `Canvas` nếu chưa có
3. Bật `Override Sorting`
4. Đặt `Sorting Order` = -1 (hoặc giá trị âm khác)
5. Đặt `Sorting Order` của UI elements = 0 hoặc dương

#### Cách 3: Sử dụng Sibling Index
```csharp
// Di chuyển background ra phía sau
backgroundImage.transform.SetAsFirstSibling();

// Di chuyển background ra phía trước
backgroundImage.transform.SetAsLastSibling();

// Đặt vị trí cụ thể (0 = đầu tiên, hiển thị phía sau)
backgroundImage.transform.SetSiblingIndex(0);
```

## Nguyên tắc hoạt động

### Thứ tự hiển thị trong Unity UI:
1. **Hierarchy Order**: Element đầu tiên hiển thị phía sau, element cuối hiển thị phía trước
2. **Sorting Order**: Giá trị thấp hiển thị phía sau, giá trị cao hiển thị phía trước
3. **Canvas Override**: Khi bật Override Sorting, element sẽ sử dụng Sorting Order riêng

### Các giá trị Sorting Order khuyến nghị:
- **Background**: -1 đến -10
- **UI Elements**: 0 đến 10
- **Popups/Modals**: 10 đến 20
- **Tooltips**: 20 đến 30

## Troubleshooting

### Background vẫn hiển thị phía trên:
1. Kiểm tra Sorting Order của background có âm không
2. Kiểm tra vị trí trong Hierarchy (background phải ở đầu)
3. Đảm bảo UI elements có Sorting Order cao hơn

### Background không hiển thị:
1. Kiểm tra Image component có sprite không
2. Kiểm tra RectTransform có anchor và offset đúng không
3. Kiểm tra Canvas có bật Override Sorting không

### UI elements bị che khuất:
1. Tăng Sorting Order của UI elements
2. Di chuyển UI elements xuống dưới trong Hierarchy
3. Đảm bảo UI elements có Canvas riêng với Sorting Order cao hơn

## Ví dụ code

```csharp
// Setup background layer
public void SetupBackground(Image backgroundImage)
{
    // 1. Di chuyển ra phía sau
    backgroundImage.transform.SetAsFirstSibling();
    
    // 2. Thêm Canvas component
    Canvas bgCanvas = backgroundImage.GetComponent<Canvas>();
    if (bgCanvas == null)
        bgCanvas = backgroundImage.gameObject.AddComponent<Canvas>();
    
    // 3. Đặt sorting order
    bgCanvas.overrideSorting = true;
    bgCanvas.sortingOrder = -1;
    
    // 4. Stretch to fill
    RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
    bgRect.anchorMin = Vector2.zero;
    bgRect.anchorMax = Vector2.one;
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;
}
```

