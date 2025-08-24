# 🐰 RabbitMQ Configuration - PlanbookAI

## Tổng quan

RabbitMQ được sử dụng làm Message Queue system cho PlanbookAI, hỗ trợ giao tiếp bất đồng bộ giữa các microservices.

## Cấu hình

### Thông tin kết nối
- **Host**: localhost (development) / rabbitmq (docker)
- **Port**: 5672 (AMQP) / 15672 (Management UI)
- **Username**: admin
- **Password**: planbookai2024
- **Virtual Host**: /

### Management UI
- **URL**: http://localhost:15672
- **Username**: admin
- **Password**: planbookai2024

## Queues

### 1. OCR Processing Queue
- **Name**: `planbookai_ocr_processing_queue`
- **Routing Key**: `ocr.processing`
- **Purpose**: Xử lý OCR requests từ ExamService

### 2. Grading Results Queue
- **Name**: `planbookai_grading_results_queue`
- **Routing Key**: `grading.results`
- **Purpose**: Nhận kết quả chấm bài từ OCR Service

### 3. AI Processing Queue
- **Name**: `planbookai_ai_processing_queue`
- **Routing Key**: `ai.processing`
- **Purpose**: Theo dõi trạng thái AI processing

### 4. AI Requests Queue
- **Name**: `planbookai_ai_requests_queue`
- **Routing Key**: `ai.requests`
- **Purpose**: Xử lý yêu cầu AI từ Gateway

### 5. Logging Queue
- **Name**: `planbookai_logging_queue`
- **Routing Key**: `logging.*`
- **Purpose**: Tập trung logging từ tất cả services

### 6. Metrics Queue
- **Name**: `planbookai_metrics_queue`
- **Routing Key**: `metrics.*`
- **Purpose**: Thu thập metrics từ tất cả services

### 7. Exam Updates Queue
- **Name**: `planbookai_exam_updates_queue`
- **Routing Key**: `exam.*`
- **Purpose**: Thông báo cập nhật đề thi

### 8. File Operations Queue
- **Name**: `planbookai_file_operations_queue`
- **Routing Key**: `file.*`
- **Purpose**: Theo dõi file operations

### 9. Notification Queue
- **Name**: `planbookai_notification_queue`
- **Routing Key**: `notification.*`
- **Purpose**: Gửi thông báo

### 10. Dead Letter Queue
- **Name**: `planbookai_dead_letter_queue`
- **Purpose**: Xử lý messages thất bại
- **TTL**: 24 giờ
- **Max Length**: 10,000 messages

## Exchanges

### 1. Main Exchange
- **Name**: `planbookai_exchange`
- **Type**: Topic
- **Purpose**: Exchange chính cho tất cả messages

### 2. Direct Exchange
- **Name**: `planbookai_direct_exchange`
- **Type**: Direct
- **Purpose**: Exchange cho direct routing

## Message Flow Examples

### OCR Processing Flow
```
ExamService → planbookai_exchange (routing: exam.ocr_request) → planbookai_ocr_processing_queue → OCRService
```

### AI Processing Flow
```
Gateway → planbookai_exchange (routing: ai.request) → planbookai_ai_requests_queue → AiPlanService
```

### Logging Flow
```
Any Service → planbookai_exchange (routing: logging.info) → planbookai_logging_queue → LogService
```

## Health Check

RabbitMQ container có health check tự động:
```bash
# Kiểm tra trạng thái
docker ps | grep rabbitmq

# Xem logs
docker logs planbookai_rabbitmq

# Kiểm tra health check
docker exec planbookai_rabbitmq rabbitmq-diagnostics ping
```

## Troubleshooting

### 1. Container không khởi động
```bash
# Kiểm tra logs
docker logs planbookai_rabbitmq

# Kiểm tra port conflicts
netstat -an | grep 5672
netstat -an | grep 15672
```

### 2. Không thể kết nối từ services
- Kiểm tra `depends_on` trong docker-compose
- Đảm bảo RabbitMQ đã healthy trước khi services khởi động
- Kiểm tra network connectivity

### 3. Messages bị mất
- Kiểm tra queue durability (đã set `durable: true`)
- Kiểm tra dead letter queue
- Xem queue statistics trong Management UI

## Development Commands

```bash
# Khởi động RabbitMQ riêng lẻ
docker run -d --name planbookai_rabbitmq_dev \
  -p 5672:5672 -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=planbookai2024 \
  rabbitmq:3-management-alpine

# Dừng và xóa container
docker stop planbookai_rabbitmq_dev
docker rm planbookai_rabbitmq_dev

# Xem queue status
docker exec planbookai_rabbitmq rabbitmqctl list_queues

# Xem exchanges
docker exec planbookai_rabbitmq rabbitmqctl list_exchanges
```

## Production Considerations

1. **Clustering**: Sử dụng RabbitMQ clustering cho high availability
2. **Persistent Storage**: Mount persistent volumes cho production
3. **Monitoring**: Tích hợp với Prometheus/Grafana
4. **Backup**: Backup định kỳ queue definitions và data
5. **Security**: Sử dụng SSL/TLS và strong passwords
6. **Resource Limits**: Set memory và CPU limits phù hợp
