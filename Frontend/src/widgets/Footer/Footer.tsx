import styles from "./Footer.module.css"

const Footer = () => {
  return (
    <footer className={styles.startFooter}>
      <img
        src="/Footer.svg"
        alt="Footer"
      />
      {/*<div className={styles.creators}>*/}
      {/*  <p>Разработано командой: <strong>MusicJam</strong> © 2026</p>*/}
      {/*  <a href="https://github.com/dmitry-veselkov/music_jam" target="_blank" className="github-link">GitHub*/}
      {/*    проекта</a>*/}
      {/*  /!*<img src="/FooterPhrase.svg" alt="Музыка нас связала" width="1100"/>*!/*/}
      {/*</div>*/}
    </footer>
  );
};

export default Footer;