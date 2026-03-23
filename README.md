# Bubble-fish-tank

## Bước 1: Tạo server websocket (có thể bỏ qua nếu có sẵn server)
Mở Terminal ở Assets\Scripts\Websocket>, chạy lệnh: python ws_server.py
Lưu ý: trên máy phải cài sẵn python và các thư viện cần thiết.

Nếu kết nối nhiều thiết bị, cần mở chạy ipconfig để lấy IPv4 của máy chạy server.

## Bước 2: Chạy Unity
Mở project.
Tại cửa sổ Project, mở thư mục Asset/Scenes, nháy đúp để mở TestWebsocketScene.
Chọn WebsocketManager ở Hierachy, điền Url của websocket server vào. (Nếu chạy server cùng một máy thì không cần).

Nhấn nút Play ở top-center màn hình.
Bắt đầu gửi tin nhắn test.

Để test tại Unity Editor, nhấn nút: "send message" trên màn hình.
