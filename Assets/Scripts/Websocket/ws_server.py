import asyncio
import websockets

connected_clients = set()

async def handler(websocket):
    connected_clients.add(websocket)
    addr = websocket.remote_address
    print(f"[+] Kết nối mới: {addr} | Tổng: {len(connected_clients)}")
    try:
        async for message in websocket:
            size = len(message)
            print(f"[→] Nhận từ {addr}: {size} bytes")
            # Broadcast đến tất cả client khác (Unity)
            others = connected_clients - {websocket}
            if others:
                await asyncio.gather(*[c.send(message) for c in others])
                print(f"[←] Đã chuyển tiếp đến {len(others)} client")
    except websockets.exceptions.ConnectionClosed:
        pass
    finally:
        connected_clients.discard(websocket)
        print(f"[-] Ngắt kết nối: {addr} | Còn lại: {len(connected_clients)}")

async def main():
    print("=" * 40)
    print("  WebSocket Server đang chạy")
    print("  URL: ws://192.168.1.120:8080")
    print("  Ctrl+C để dừng")
    print("=" * 40)
    async with websockets.serve(handler, "0.0.0.0", 8080):
        await asyncio.Future()

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\nServer đã dừng.")

# Câu lệnh:
# python ws_server.py