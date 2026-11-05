import Link from "next/link";

export default function Footer() {
    return (
        <footer className="site-footer">
            <Link className="brand brand-footer" href="/">
                <span className="brand-mark" aria-hidden="true">H</span>
                <span>HORIZONTE <span className="brand-light">HOTELES</span></span>
            </Link>
            <span>Viaja a tu manera. Nosotros nos ocupamos del resto.</span>
        </footer>
    )
}