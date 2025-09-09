# Hướng dẫn sử dụng Typing Effect cho Lilith Chat

## 📝 Tổng quan

Hệ thống typing effect cho phép text của Lilith hiển thị từng chữ một thay vì hiện ra cả đoạn cùng lúc, tạo cảm giác tự nhiên và sinh động hơn.

## 🎯 Các tính năng chính

### TextTypingEffect.cs
- **Typing Speed**: Điều chỉnh tốc độ hiển thị từng ký tự
- **Punctuation Pause**: Tạm dừng ở dấu câu để tạo nhịp điệu tự nhiên
- **Cursor Blinking**: Hiển thị con trỏ nhấp nháy trong khi typing
- **Skip on Click**: Bỏ qua typing khi click chuột hoặc nhấn Space
- **Audio Support**: Hỗ trợ âm thanh typing (tùy chọn)

### ChatTypingManager.cs
- **Message Management**: Quản lý hiển thị tin nhắn với typing effect
- **Speed Control**: Điều chỉnh tốc độ typing khác nhau
- **Typing Indicator**: Hiển thị "Lilith đang nhập..." khi đang typing
- **Integration**: Tích hợp dễ dàng với ChatManager hiện tại

## 🚀 Cách sử dụng

### Bước 1: Thiết lập cơ bản

1. **Thêm TextTypingEffect vào GameObject chứa TMP_Text:**
   ```
   - Chọn GameObject chứa TMP_Text (transcriptText)
   - Add Component > TextTypingEffect
   - Gán TMP_Text vào field "Target Text"
   ```

2. **Thêm ChatTypingManager:**
   ```
   - Chọn GameObject chứa ChatManager hoặc tạo GameObject mới
   - Add Component > ChatTypingManager
   - Gán references:
     + Transcript Text: TMP_Text hiển thị chat
     + Typing Effect: TextTypingEffect component
   ```

### Bước 2: Cấu hình

#### TextTypingEffect Settings:
```
Typing Speed: 0.05f (thời gian giữa mỗi ký tự)
Pause At Punctuation: 0.3f (tạm dừng ở dấu câu)
Auto Start: false (không tự động bắt đầu)
Skip On Click: true (bỏ qua khi click)
Show Cursor: true (hiển thị con trỏ)
Cursor Char: "|" (ký tự con trỏ)
```

#### ChatTypingManager Settings:
```
Enable Typing Effect: true
Default Typing Speed: 0.05f
Fast Typing Speed: 0.02f
Show Typing Indicator: true
```

### Bước 3: Sử dụng trong code

#### Cách 1: Sử dụng ChatTypingManager (Khuyến nghị)
```csharp
// Lấy reference
ChatTypingManager typingManager = FindObjectOfType<ChatTypingManager>();

// Hiển thị tin nhắn với typing effect
typingManager.DisplayMessageWithTyping("Xin chào! Tôi là Lilith...", true);

// Hiển thị tin nhắn nhanh
typingManager.DisplayMessageFast("Tin nhắn nhanh!");

// Hiển thị tin nhắn chậm
typingManager.DisplayMessageSlow("Tin nhắn chậm...");

// Thêm tin nhắn vào cuối
typingManager.AppendMessageWithTyping("\nTin nhắn mới", true);

// Bỏ qua typing hiện tại
typingManager.SkipCurrentTyping();
```

#### Cách 2: Sử dụng TextTypingEffect trực tiếp
```csharp
// Lấy reference
TextTypingEffect typingEffect = FindObjectOfType<TextTypingEffect>();

// Bắt đầu typing
typingEffect.StartTyping("Xin chào! Tôi là Lilith...");

// Thiết lập tốc độ
typingEffect.SetTypingSpeed(0.03f);

// Bỏ qua typing
typingEffect.SkipTyping();
```

### Bước 4: Tích hợp với ChatManager

Để tích hợp với ChatManager hiện tại, thêm code sau vào ChatManager:

```csharp
[Header("Typing Effect")]
[SerializeField] private ChatTypingManager typingManager;

private void Start()
{
    // Tìm ChatTypingManager nếu chưa được gán
    if (typingManager == null)
    {
        typingManager = FindObjectOfType<ChatTypingManager>();
    }
}

// Thay thế cách hiển thị tin nhắn
private void DisplayLilithMessage(string message)
{
    if (typingManager != null)
    {
        typingManager.AppendMessageWithTyping(message, true);
    }
    else
    {
        // Fallback: hiển thị bình thường
        transcriptText.text += "\n" + message;
    }
}
```

## 🎨 Tùy chỉnh nâng cao

### 1. Tạo Typing Indicator UI
```
- Tạo GameObject mới với TMP_Text
- Đặt tên "TypingIndicator"
- Gán vào ChatTypingManager field "Typing Indicator"
- Ẩn GameObject này ban đầu
```

### 2. Thêm âm thanh typing
```
- Tạo AudioSource component
- Import file âm thanh typing (.wav, .mp3)
- Gán vào TextTypingEffect:
  + Audio Source: AudioSource component
  + Typing Sound: AudioClip file
  + Typing Volume: 0.3f
```

### 3. Tùy chỉnh cursor
```
- Thay đổi Cursor Char: "█", "▋", "▌", "▍", "▎", "▏"
- Điều chỉnh Cursor Blink Speed: 0.5f
- Tắt/bật Show Cursor
```

## 🔧 Troubleshooting

### Vấn đề thường gặp:

1. **Text không hiển thị typing effect:**
   - Kiểm tra Enable Typing Effect = true
   - Đảm bảo Target Text được gán đúng
   - Kiểm tra IsLilithMessage = true

2. **Typing quá nhanh/chậm:**
   - Điều chỉnh Typing Speed (0.01f - 0.1f)
   - Sử dụng Fast/Slow typing methods

3. **Cursor không nhấp nháy:**
   - Kiểm tra Show Cursor = true
   - Điều chỉnh Cursor Blink Speed

4. **Không thể bỏ qua typing:**
   - Kiểm tra Skip On Click = true
   - Click chuột hoặc nhấn Space

## 📋 Test Commands

Sử dụng Context Menu để test:
- **Test Typing Effect**: Test typing cơ bản
- **Test Fast Typing**: Test typing nhanh
- **Test Typing Indicator**: Test indicator
- **Skip Current Typing**: Bỏ qua typing hiện tại

## 🎯 Tips

1. **Tốc độ typing phù hợp:**
   - Chậm (0.05f - 0.08f): Tạo cảm giác thư thái
   - Trung bình (0.03f - 0.05f): Tự nhiên
   - Nhanh (0.01f - 0.03f): Năng động

2. **Punctuation pause:**
   - Dấu chấm (.): 0.3f - 0.5f
   - Dấu phẩy (,): 0.2f - 0.3f
   - Dấu chấm than (!): 0.4f - 0.6f

3. **Performance:**
   - Tắt typing effect khi có nhiều tin nhắn
   - Sử dụng Skip typing cho tin nhắn dài
   - Tối ưu hóa audio nếu sử dụng

## 🔗 Tích hợp với hệ thống hiện tại

Hệ thống typing effect được thiết kế để hoạt động độc lập và có thể tích hợp dễ dàng với:
- ChatManager.cs
- ChatUI.cs
- SimpleScrollFix.cs
- BackgroundLayerManager.cs

Chỉ cần thay thế cách hiển thị text từ `transcriptText.text = message` thành `typingManager.DisplayMessageWithTyping(message, true)`.
