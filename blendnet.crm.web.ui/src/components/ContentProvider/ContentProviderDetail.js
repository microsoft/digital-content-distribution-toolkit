import React, { Component } from 'react';
import CPDetailForm from './CPDetailForm';

export class ContentProviderDetail extends Component {
  static displayName = ContentProviderDetail.name;

  render () {
    return (
      <div>
          <CPDetailForm></CPDetailForm>
      </div>
    );
  }
}
