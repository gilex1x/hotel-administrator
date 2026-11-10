const MAX_IMAGE_BYTES = 400 * 1024;
const MAX_JPEG_QUALITY = 1;
const MIN_JPEG_QUALITY = 0.1;

function canvasToJpeg(canvas: HTMLCanvasElement, quality: number): Promise<Blob> {
  return new Promise((resolve, reject) => {
    canvas.toBlob((blob) => {
      if (blob) {
        resolve(blob);
      } else {
        reject(new Error("No se pudo procesar la imagen seleccionada."));
      }
    }, "image/jpeg", quality);
  });
}

function blobToDataUri(blob: Blob): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      if (typeof reader.result === "string") {
        resolve(reader.result);
      } else {
        reject(new Error("No se pudo leer la imagen procesada."));
      }
    };
    reader.onerror = () => reject(new Error("No se pudo leer la imagen procesada."));
    reader.readAsDataURL(blob);
  });
}

export async function compressHotelImage(file: File): Promise<string> {
  const objectUrl = URL.createObjectURL(file);
  try {
    const image = new Image();
    image.src = objectUrl;
    await image.decode();

    const canvas = document.createElement("canvas");
    let width = image.naturalWidth;
    let height = image.naturalHeight;

    while (width > 0 && height > 0) {
      canvas.width = width;
      canvas.height = height;
      const context = canvas.getContext("2d");
      if (!context) {
        throw new Error("El navegador no pudo preparar la imagen.");
      }
      context.drawImage(image, 0, 0, width, height);

      const smallest = await canvasToJpeg(canvas, MIN_JPEG_QUALITY);
      if (smallest.size <= MAX_IMAGE_BYTES) {
        let low = MIN_JPEG_QUALITY;
        let high = MAX_JPEG_QUALITY;
        let best = smallest;

        for (let attempt = 0; attempt < 8; attempt += 1) {
          const quality = (low + high) / 2;
          const candidate = await canvasToJpeg(canvas, quality);
          if (candidate.size <= MAX_IMAGE_BYTES) {
            best = candidate;
            low = quality;
          } else {
            high = quality;
          }
        }

        return blobToDataUri(best);
      }

      if (width === 1 && height === 1) {
        break;
      }

      const scale = Math.min(0.85, 2048 / Math.max(width, height));
      width = Math.max(1, Math.floor(width * scale));
      height = Math.max(1, Math.floor(height * scale));
    }

    throw new Error("No se pudo reducir la imagen a 400 KB.");
  } finally {
    URL.revokeObjectURL(objectUrl);
  }
}
