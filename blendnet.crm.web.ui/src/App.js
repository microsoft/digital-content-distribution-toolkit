import React from 'react';
import clsx from 'clsx';
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import Divider from '@material-ui/core/Divider';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import AccountCircle from '@material-ui/icons/AccountCircle';
import Container from '@material-ui/core/Container';
import MenuItem from "@material-ui/core/MenuItem";
import Menu from "@material-ui/core/Menu";
import './custom.css';
import { ContentProviderList } from './components/ContentProvider/ContentProviderList';
import { HubList } from './components/Hubs/HubList';
import { Profile } from './components/User/Profile';
import {Home} from './components/Home';
import {Policies} from './components/Policy/Policies';
import Tooltip from '@material-ui/core/Tooltip';
import { ContentProviderDetail } from './components/ContentProvider/ContentProviderDetail';
import { Route } from 'react-router';
import { Link } from 'react-router-dom';
import HomeIcon from '@material-ui/icons/Home';
import AirplayIcon from '@material-ui/icons/Airplay';
import TheatersIcon from '@material-ui/icons/Theaters';
import DeviceHubIcon from '@material-ui/icons/DeviceHub';
import PolicyIcon from '@material-ui/icons/Policy';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    backgroundColor: "#0078d4",
  },
  appBarShift: {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  menuButton: {
    marginRight: 36,
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
    whiteSpace: 'nowrap',
  },
  drawerOpen: {
    width: drawerWidth,
    transition: theme.transitions.create('width', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  drawerClose: {
    transition: theme.transitions.create('width', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    width: theme.spacing(7) + 1,
    [theme.breakpoints.up('sm')]: {
      width: theme.spacing(9) + 1,
    },
  },
  toolbar: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
  title: {
    flexGrow: 1
  },
  link: {
    textDecoration: 'none',
    color: 'grey'
  },
  profileLink: {
    textDecoration: 'none',
    color: '#ffffff'
  }
}));

export default function MiniDrawer() {
  const classes = useStyles();
  const theme = useTheme();
  const [open, setOpen] = React.useState(false);

  const handleDrawerOpen = () => {
    setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };


  return (
    <div className={classes.root}>
      <CssBaseline />
      <AppBar
        position="fixed"
        className={clsx(classes.appBar, {
          [classes.appBarShift]: open,
        })}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            onClick={handleDrawerOpen}
            edge="start"
            className={clsx(classes.menuButton, {
              [classes.hide]: open,
            })}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap className={classes.title}>
              Bine CRM
          </Typography>
          <div>
          <Link to="/profile" className={classes.profileLink}>
            <IconButton
                  aria-label="account of current user"
                  aria-controls="menu-appbar"
                  aria-haspopup="true"
                  color="inherit"
                >
                  <AccountCircle />
              </IconButton>
          </Link>
            </div>
          </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        className={clsx(classes.drawer, {
          [classes.drawerOpen]: open,
          [classes.drawerClose]: !open,
        })}
        classes={{
          paper: clsx({
            [classes.drawerOpen]: open,
            [classes.drawerClose]: !open,
          }),
        }}
      >
       <div className={classes.toolbar}>
          <IconButton onClick={handleDrawerClose}>
            <ChevronLeftIcon />
          </IconButton>
        </div>
        <Divider />
        <List>
          <Link to="/home" className={classes.link}>
            <ListItem button key="Home">
              <ListItemIcon><HomeIcon /> </ListItemIcon>
              <ListItemText primary="Home" />
            </ListItem>
          </Link>
        </List>
        <List>
          <Link to="/content-provide-list" className={classes.link}>
            <ListItem button key="ContentProviders">
              <ListItemIcon><AirplayIcon /> </ListItemIcon>
              <ListItemText primary="Content-Providers" />
            </ListItem>
          </Link>
        </List>
        <List>
          <Link to="/content-provider-detail" className={classes.link}>
            <ListItem button key="Content">
              <ListItemIcon><TheatersIcon /> </ListItemIcon>
              <ListItemText primary="Content" />
            </ListItem>
          </Link>
        </List>
        <List>
          <Link  className={classes.link} to="/hubs">
            <ListItem button key="Hubs">
              <ListItemIcon><DeviceHubIcon /> </ListItemIcon>
              <ListItemText primary="Hubs" />
            </ListItem>
          </Link>
        </List>
        <List>
          <Link className={classes.link} to="/policies">
            <ListItem button key="Policies">
              <ListItemIcon><PolicyIcon /> </ListItemIcon>
              <ListItemText primary="Policies" />
            </ListItem>
          </Link>
        </List>
      </Drawer>
      <main className={classes.content}>
        <div className={classes.toolbar} />
        <Container className={classes.container}>
            <Route path='/content-provide-list' component={ContentProviderList} />
            <Route path='/hubs' component={HubList} />
            <Route path='/profile' component={Profile} />
            <Route path='/home' component={Home} />
            <Route path='/policies' component={Policies} />
            <Route path='/content-provider-detail' component={ContentProviderDetail} />
          </Container>
      </main>
    </div>
  );
}
