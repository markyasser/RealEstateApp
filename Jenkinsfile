pipeline {
    agent any

    environment {
        APP_NAME = "realestate"
        DOCKER_IMAGE_NAME = "${env.APP_NAME}:${env.BUILD_ID}"
        SERVER = "mark@51.103.246.195"
        DOCKER_COMPOSE_PATH = "~/docker-compose.yml"
    }

    stages {
        stage('Build') {
            steps {
                script {
                    // Build the Docker image
                    docker.build(env.DOCKER_IMAGE_NAME)
                }
            }
        }

        stage('Deploy') {
            steps {
                sshagent(['your_ssh_credentials']) {
                    // Save the Docker image and transfer it to the server
                    sh """
                    docker save ${env.DOCKER_IMAGE_NAME} | bzip2 | ssh -o StrictHostKeyChecking=no ${env.SERVER} 'bunzip2 | docker load'
                    """

                    // Transfer docker-compose.yml to the server
                    sh """
                    scp -o StrictHostKeyChecking=no docker-compose.yml ${env.SERVER}:${env.DOCKER_COMPOSE_PATH}
                    """

                    // Deploy using Docker Compose on the server
                    sh """
                    ssh -o StrictHostKeyChecking=no ${env.SERVER} '
                        export COMPOSE_PROJECT_NAME=${env.APP_NAME}
                        docker-compose -f ${env.DOCKER_COMPOSE_PATH} down
                        docker-compose -f ${env.DOCKER_COMPOSE_PATH} up -d --build
                    '
                    """
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}
