# Bubble-fish-tank
Hoàn thành chức năng cơ bản: app/web gửi ảnh và lời nhắn qua web socket,tại Unity game một bong bóng bay lên, chạm vào mặt nước, hiển thị lời nhắn và làm mặt nước sạch hơn.

https://github.com/user-attachments/assets/399c386f-4655-43b2-ab56-6c6792bafd4f


## Hướng dẫn chạy
### Bước 1: Tạo server websocket (có thể bỏ qua nếu có sẵn server)
Mở Terminal ở Assets\Scripts\Websocket>, chạy lệnh: python ws_server.py
Lưu ý: trên máy phải cài sẵn python và các thư viện cần thiết.

Nếu kết nối nhiều thiết bị, cần mở chạy ipconfig để lấy IPv4 của máy chạy server.

### Bước 2: Chạy Unity
Mở project.
Tại cửa sổ Project, mở thư mục Asset/Scenes, nháy đúp để mở TestWebsocket2Scene.unity.
Chọn WebsocketManager ở Hierachy, điền Url của websocket server vào. (Nếu chạy server cùng một máy thì không cần).

Nhấn nút Play ở top-center màn hình.
Bắt đầu gửi tin nhắn test.

Để test tại Unity Editor, nhấn nút: "send message" trên màn hình.

## Lưu ý
Sửa Dirty Water Volume và Clean Water Volume để chỉnh màu nước trước và sau khi có bong bóng.
