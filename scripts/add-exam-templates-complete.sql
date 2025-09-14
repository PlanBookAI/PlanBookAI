-- =====================================================
-- MIGRATION SCRIPT: Thêm tính năng quản lý mẫu đề thi
-- Ngày tạo: $(date)
-- Mô tả: Thêm bảng exam_templates và các constraints liên quan
-- =====================================================

BEGIN;

-- 1. Tạo bảng assessment.exam_templates
CREATE TABLE assessment.exam_templates (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    title character varying(255) NOT NULL,
    description text,
    subject character varying(100) DEFAULT 'HOA_HOC'::character varying NOT NULL,
    grade integer,
    duration_minutes integer,
    total_score numeric(5,2),
    structure jsonb, -- Cấu trúc đề thi (số câu hỏi theo độ khó, chủ đề)
    created_by uuid NOT NULL,
    status character varying(50) DEFAULT 'ACTIVE'::character varying,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT exam_templates_duration_minutes_check CHECK ((duration_minutes > 0)),
    CONSTRAINT exam_templates_grade_check CHECK ((grade = ANY (ARRAY[10, 11, 12]))),
    CONSTRAINT exam_templates_status_check CHECK (((status)::text = ANY (ARRAY[('ACTIVE'::character varying)::text, ('INACTIVE'::character varying)::text, ('ARCHIVED'::character varying)::text]))),
    CONSTRAINT exam_templates_total_score_check CHECK ((total_score > (0)::numeric))
);

-- 2. Set owner
ALTER TABLE assessment.exam_templates OWNER TO test;

-- 3. Primary key
ALTER TABLE ONLY assessment.exam_templates
    ADD CONSTRAINT exam_templates_pkey PRIMARY KEY (id);

-- 4. Foreign key constraints
ALTER TABLE ONLY assessment.exam_templates
    ADD CONSTRAINT exam_templates_created_by_fkey FOREIGN KEY (created_by) REFERENCES auth.users(id);

-- 5. Indexes để tối ưu performance
CREATE INDEX idx_assessment_exam_templates_subject ON assessment.exam_templates USING btree (subject);
CREATE INDEX idx_assessment_exam_templates_created_by ON assessment.exam_templates USING btree (created_by);
CREATE INDEX idx_assessment_exam_templates_status ON assessment.exam_templates USING btree (status);
CREATE INDEX idx_assessment_exam_templates_grade ON assessment.exam_templates USING btree (grade);

-- 6. Trigger để tự động cập nhật updated_at
CREATE TRIGGER trigger_updated_at_assessment_exam_templates 
    BEFORE UPDATE ON assessment.exam_templates 
    FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

-- 7. Thêm cột template_id vào bảng exams để liên kết với mẫu đề thi
ALTER TABLE assessment.exams 
ADD COLUMN template_id uuid;

-- 8. Foreign key constraint cho template_id
ALTER TABLE ONLY assessment.exams
    ADD CONSTRAINT exams_template_id_fkey FOREIGN KEY (template_id) REFERENCES assessment.exam_templates(id);

-- 9. Index cho template_id
CREATE INDEX idx_assessment_exams_template ON assessment.exams USING btree (template_id);

-- 10. Thêm comment cho bảng và cột
COMMENT ON TABLE assessment.exam_templates IS 'Bảng lưu trữ các mẫu đề thi để tái sử dụng';
COMMENT ON COLUMN assessment.exam_templates.structure IS 'Cấu trúc đề thi dạng JSON: {"easy": 5, "medium": 10, "hard": 3, "topics": ["topic1", "topic2"]}';
COMMENT ON COLUMN assessment.exams.template_id IS 'ID của mẫu đề thi được sử dụng để tạo đề thi này';

-- 11. Thêm dữ liệu mẫu (optional)
INSERT INTO assessment.exam_templates (
    id, title, description, subject, grade, duration_minutes, total_score, 
    structure, created_by, status
) VALUES (
    gen_random_uuid(),
    'Mẫu đề thi Hóa học lớp 10 - Cơ bản',
    'Mẫu đề thi cơ bản cho học sinh lớp 10 môn Hóa học',
    'HOA_HOC',
    10,
    45,
    10.0,
    '{"easy": 8, "medium": 6, "hard": 2, "topics": ["Cấu trúc nguyên tử", "Bảng tuần hoàn"]}',
    '550e8400-e29b-41d4-a716-446655440002', -- teacher@test.com
    'ACTIVE'
), (
    gen_random_uuid(),
    'Mẫu đề thi Hóa học lớp 11 - Nâng cao',
    'Mẫu đề thi nâng cao cho học sinh lớp 11 môn Hóa học',
    'HOA_HOC',
    11,
    60,
    10.0,
    '{"easy": 5, "medium": 8, "hard": 4, "topics": ["Liên kết hóa học", "Phản ứng hóa học"]}',
    '550e8400-e29b-41d4-a716-446655440002',
    'ACTIVE'
);

COMMIT;

-- =====================================================
-- VERIFICATION QUERIES
-- Chạy các query này để kiểm tra migration thành công
-- =====================================================

-- Kiểm tra bảng đã được tạo
SELECT table_name, table_schema 
FROM information_schema.tables 
WHERE table_name = 'exam_templates' AND table_schema = 'assessment';

-- Kiểm tra cột template_id đã được thêm vào exams
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'exams' AND table_schema = 'assessment' AND column_name = 'template_id';

-- Kiểm tra dữ liệu mẫu
SELECT id, title, subject, grade, status FROM assessment.exam_templates;

-- Kiểm tra constraints
SELECT constraint_name, constraint_type 
FROM information_schema.table_constraints 
WHERE table_name = 'exam_templates' AND table_schema = 'assessment';
