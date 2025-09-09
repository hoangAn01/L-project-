# 🎯 Hướng dẫn gắn Typing Effect cho Lilith Chat

## 📋 Tổng quan

Hướng dẫn này sẽ giúp bạn gắn và thiết lập hệ thống typing effect cho Lilith Chat một cách hoàn chỉnh.

## ✅ Kiểm tra setup hiện tại

Từ Unity Editor, tôi thấy bạn đã có:
- ✅ `ChatTypingManager` đã được gắn vào `TranscriptTMP`
- ✅ `Transcript Text` đã được gán: `TranscriptTMP (Text Mesh Pro UGUI)`
- ✅ `Typing Effect` đã được gán: `TranscriptTMP (Text Typing Effect)`
- ✅ `Enable Typing Effect`: đã bật
- ✅ `Default Typing Speed`: 0.05
- ✅ `Fast Typing Speed`: 0.02

## 🚀 Bước 1: Thiết lập Typing Indicator (Tùy chọn)

### Tạo Typing Indicator GameObject:

1. **Trong Hierarchy:**
   ```
   - Chọn Canvas
   - Right-click > UI > Text - TextMeshPro
   - Đặt tên: "TypingIndicator"
   ```

2. **Thiết lập Typing Indicator:**
   ```
   - Chọn TypingIndicator GameObject
   - Trong TextMeshPro component:
     + Text: "Lilith đang nhập..."
     + Font Size: 18
     + Color: Gray (#808080)
     + Alignment: Left
   - Ẩn GameObject này ban đầu (uncheck "Active")
   ```

3. **Gán vào ChatTypingManager:**
   ```
   - Chọn ChatTypingManager trong Hierarchy
   - Trong Inspector, kéo TypingIndicator vào field "Typing Indicator"
   ```

## 🔧 Bước 2: Thêm ChatTypingIntegration

### Tạo Integration Script:

1. **Thêm component:**
   ```
   - Chọn GameObject chứa ChatManager hoặc tạo GameObject mới
   - Add Component > ChatTypingIntegration
   ```

2. **Thiết lập references:**
   ```
   - Chat Typing Manager: kéo ChatTypingManager từ Hierarchy
   - Chat UI: kéo ChatUI từ Hierarchy
   - Chat Manager: kéo ChatManager từ Hierarchy
   ```

3. **Cấu hình settings:**
   ```
   - Enable Typing For Lilith: true (bật typing cho Lilith)
   - Enable Typing For User: false (tắt typing cho user)
   ```

## 🎮 Bước 3: Test Typing Effect

### Sử dụng Context Menu:

1. **Test cơ bản:**
   ```
   - Chọn ChatTypingManager trong Hierarchy
   - Right-click > Test Typing Effect
   ```

2. **Test các loại typing:**
   ```
   - Test Fast Typing: typing nhanh
   - Test Typing Indicator: hiển thị indicator
   - Skip Current Typing: bỏ qua typing
   ```

3. **Test từ ChatTypingIntegration:**
   ```
   - Test Lilith Message: tin nhắn của Lilith
   - Test User Message: tin nhắn của User
   - Test Fast Message: tin nhắn nhanh
   - Test Slow Message: tin nhắn chậm
   ```

## 🔗 Bước 4: Tích hợp với ChatManager

### Cách 1: Sử dụng ChatTypingIntegration (Khuyến nghị)

```csharp
// Lấy reference
ChatTypingIntegration integration = FindObjectOfType<ChatTypingIntegration>();

// Hiển thị tin nhắn của Lilith với typing effect
integration.DisplayLilithMessage("Xin chào! Tôi là Lilith...");

// Hiển thị tin nhắn của User
integration.DisplayUserMessage("Xin chào Lilith!");

// Hiển thị tin nhắn nhanh
integration.DisplayMessageFast("Tin nhắn nhanh!");

// Bỏ qua typing hiện tại
integration.SkipCurrentTyping();
```

### Cách 2: Sử dụng ChatTypingManager trực tiếp

```csharp
// Lấy reference
ChatTypingManager typingManager = FindObjectOfType<ChatTypingManager>();

// Hiển thị tin nhắn với typing effect
typingManager.DisplayMessageWithTyping("Xin chào! Tôi là Lilith...", true);

// Thêm tin nhắn vào cuối
typingManager.AppendMessageWithTyping("\nTin nhắn mới", true);

// Bỏ qua typing hiện tại
typingManager.SkipCurrentTyping();
```

## 🎨 Bước 5: Tùy chỉnh nâng cao

### 1. Điều chỉnh tốc độ typing:

```csharp
// Trong Inspector hoặc code
ChatTypingManager typingManager = FindObjectOfType<ChatTypingManager>();

// Thiết lập tốc độ
typingManager.SetDefaultTypingSpeed(0.03f); // Nhanh hơn
typingManager.SetDefaultTypingSpeed(0.08f); // Chậm hơn
```

### 2. Bật/tắt typing effect:

```csharp
// Bật/tắt cho Lilith
ChatTypingIntegration integration = FindObjectOfType<ChatTypingIntegration>();
integration.SetTypingForLilith(true);  // Bật
integration.SetTypingForLilith(false); // Tắt

// Bật/tắt cho User
integration.SetTypingForUser(true);  // Bật
integration.SetTypingForUser(false); // Tắt
```

### 3. Tùy chỉnh TextTypingEffect:

```
Trong Inspector của TextTypingEffect:
- Typing Speed: 0.05f (tốc độ cơ bản)
- Pause At Punctuation: 0.3f (tạm dừng ở dấu câu)
- Show Cursor: true (hiển thị con trỏ)
- Cursor Char: "|" (ký tự con trỏ)
- Cursor Blink Speed: 0.5f (tốc độ nhấp nháy)
```

## 🔧 Bước 6: Troubleshooting

### Vấn đề thường gặp:

1. **Text không hiển thị typing effect:**
   ```
   - Kiểm tra Enable Typing Effect = true
   - Đảm bảo Target Text được gán đúng
   - Kiểm tra IsTypingEffectReady() trả về true
   ```

2. **Typing quá nhanh/chậm:**
   ```
   - Điều chỉnh Typing Speed (0.01f - 0.1f)
   - Sử dụng Fast/Slow typing methods
   ```

3. **Cursor không nhấp nháy:**
   ```
   - Kiểm tra Show Cursor = true
   - Điều chỉnh Cursor Blink Speed
   ```

4. **Không thể bỏ qua typing:**
   ```
   - Kiểm tra Skip On Click = true
   - Click chuột hoặc nhấn Space
   ```

## 📋 Checklist hoàn thành

- [ ] ChatTypingManager đã được gắn vào TranscriptTMP
- [ ] Transcript Text đã được gán đúng
- [ ] Typing Effect đã được gán đúng
- [ ] Enable Typing Effect đã được bật
- [ ] Typing Indicator đã được tạo và gán (tùy chọn)
- [ ] ChatTypingIntegration đã được thêm
- [ ] References đã được thiết lập đúng
- [ ] Test typing effect hoạt động
- [ ] Test skip typing hoạt động
- [ ] Test typing indicator hoạt động (nếu có)

## 🎯 Kết quả mong đợi

Sau khi hoàn thành setup:
- ✅ Text của Lilith sẽ hiển thị từng chữ một
- ✅ Có thể bỏ qua typing bằng click chuột hoặc Space
- ✅ Tạm dừng ở dấu câu để tạo nhịp điệu tự nhiên
- ✅ Con trỏ nhấp nháy trong khi typing
- ✅ Typing indicator hiển thị "Lilith đang nhập..." (nếu có)
- ✅ Tích hợp hoàn chỉnh với hệ thống chat hiện tại

## 🚀 Sử dụng trong game

```csharp
// Trong script khác, sử dụng:
ChatTypingIntegration integration = FindObjectOfType<ChatTypingIntegration>();

// Khi nhận được response từ AI
string aiResponse = "Xin chào! Tôi là Lilith...";
integration.DisplayLilithMessage(aiResponse);

// Khi user gửi tin nhắn
string userMessage = "Xin chào Lilith!";
integration.DisplayUserMessage(userMessage);
```

Bây giờ hệ thống typing effect đã sẵn sàng sử dụng! 🎉
